using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Hasher = SimpleBC.Hash;


namespace SimpleBC
{
    /// <summary>
    /// Class representing a single block (in a blockchain)
    /// </summary>
    class Block : INotifyPropertyChanged
    {
        private const string signKey = "0000";

        private string id;
        /// <summary>
        /// String property representing the block ID (and also its position in the blockchain)
        /// </summary>
        public string ID
        {
            get { return id; }
            set { if (id != value) { id = value; NotifyPropertyChanged(); } }
        }

        private int nonce;
        /// <summary>
        /// Integer property representing the nonce, an arbitrary value that can mined in an 
        /// attempt to make the block signed according to the set criterion
        /// (Default: Hash must start "0000.......")
        /// </summary>
        public int Nonce
        {
            get { return nonce; }
            set { nonce = value; NotifyPropertyChanged(); }
        }

        private string data;
        /// <summary>
        /// String property, arbitrary data carried by the block
        /// </summary>
        public string Data
        {
            get { return data; }
            set { data = value; NotifyPropertyChanged(); }
        }

        private string previousHash;
        /// <summary>
        /// String property, the hash of the previous block in the blockchain. Note that an 
        /// individual block has no notion of a 'blockchain' and that this property must be set 
        /// by something external - normally the chain 'manager'
        /// </summary>
        public string PreviousHash
        {
            get { return previousHash; }
            set { previousHash = value; NotifyPropertyChanged(); }
        }

        private string hash;
        /// <summary>
        /// The current hash of the ID+(string)Nonce+Data+PreviousHash concatenated without spaces,
        /// using the hashing algorithm set by the HashGenerator class.
        /// </summary>
        public string Hash
        {
            get { return hash; }
            set { hash = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Constructor for an 'empty' block based on a block id.
        /// </summary>
        /// <param name="id"></param>
        public Block(string id)
        {
            this.id = id;
            nonce = 0;
            data = String.Empty;
            previousHash = "not set";
            PropertyChanged += internalHandler;
        }

        private void internalHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Hash" || e.PropertyName == "IsSigned") return; // avoid recursion
            this.ReHash();
        }

        private void ReHash()
        {
            Hash = Hasher.HashGen(ID, Nonce.ToString(), Data, PreviousHash);
            PropertyChanged(this, new PropertyChangedEventArgs("IsSigned"));
        }

        /// <summary>
        /// Boolean property indicating whether the block currently satisfies the signing criterion
        /// </summary>
        public bool IsSigned => String.Equals(Hash.Substring(0, signKey.Length), signKey);

        /// <summary>
        /// Static flag used by blocks to ensure that 
        /// a) mining is not re-entrant 
        /// b) only one block globally can be being mined at anyone time
        /// </summary>
        /// <remarks>
        /// Note that mining would expect to run on a separate thread to the main program
        /// </remarks>
        public static bool IsMining { get; private set; } = false;

        /// <summary>
        /// INotifyPropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Void method called to 'mine' a block, that is iteratively try nonces until one is found that 
        /// causes the block's hash to satisfy the signing criterion. May take many seconds or even minutes to 
        /// return, therefore should not be called on the UI thread. 
        /// </summary>
        /// <remark>
        /// There is currently no way to interrupt this process, it must be allowed to run to completion.
        /// </remark>
        public void Mine() // note that IsMining is static so that only one block may be mining at any one time
        {
            /*real paranoia would thread-lock the next two lines two ensure that you can't
            / get unlucky with two button presses simultaneously
            / academic since you couldn't switch tabs/double click
            / fast enough anyway ...
            */

            if (IsMining) return; // prevent re-entrancy
            IsMining = true;

            nonce = 0;
            while (!IsSigned)
                Nonce += 1;

            IsMining = false;
            NotifyPropertyChanged("Hash"); //to allow blocklist to see new 'previous'
        }
    }

    /// <summary>
    /// An Observable Collection of Block.
    /// The Add method is overriden to handle property changes of the Hashes of the Blocks
    /// it contains.
    /// </summary>
    /// <remarks>
    /// Adding a Block with an ID of 0 is a special case in that its PreviousHash will be set to 
    /// the constant Hash.HashZero.
    /// </remarks>
    class BlockChain : ObservableCollection<Block>
    {
        /// <summary>
        /// Adds a block to the block chain
        /// </summary>
        /// <param name="B">The Block to add</param>
        public new void Add(Block B)
        {
            B.PreviousHash = B.ID.Equals("0") ? Hasher.HashZero : this[this.Count - 1].Hash;
            base.Add(B);
            B.PropertyChanged += internalHandler;
        }

        // Copy Hash -> PreviousHash of next block
        private void internalHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Hash") return;
            if (sender is Block B)
            {
                if (Block.IsMining) return;

                int index = int.Parse(B.ID);
                if (index + 1 == this.Count) return; //last block, nothing to do
                this[index + 1].PreviousHash = this[index].Hash;
            }
        }

        /// <summary>
        /// Utility method to return an initialized five-block blockchain
        /// </summary>
        /// <param name="CSP">The HashAlgorithm to use. If not provided will 
        /// default to SHA1CryptoServiceProvider</param>
        /// <returns>the initialzed BlockChain</returns>
        public static BlockChain GetInitializedBlockChain(HashAlgorithm CSP = null)
        {
            if (CSP != null) Hash.CSP = CSP;
            // e.g. new SHA512CryptoServiceProvider();          

            BlockChain result = new BlockChain();
            foreach (int i in Enumerable.Range(0, 5))
                result.Add(new Block(i.ToString()));
            result[0].Data = Messages.Block0Data;
            return result;
        }
    }
}


