using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleBC;
using System.Threading.Tasks;

/* INCOMPLETE - WORK IN PROGRESS */
/* ============================= */

/* Proposed test plan:
 * Create a blockchain
 * Set data to specified values
 * Mine each block sequentially
 * 
 * Test the following
 * 
 * i) there are five blocks
 * ii) Block 0 has its PreviousHash set correctly
 * iii) Blocks 1-4 have their PreviousHash set correctly
 * iv) Hashes (for selected items) are as expected
 * v) IsSigned returns true for all blocks
 * 
 */


namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        BlockChain TestObject;

        private void SetBlockChainToInitialState()
        {
            TestObject = BlockChain.GetInitializedBlockChain();
            string[] testData = new string[] { "x", "a", "b", "c", "d" };
            /* Set data */
            for (int i = 0; i < 5; i++)
            {
                TestObject[i].Data = testData[i];
            }
        }

        //Noth that this will take 11-12s - this is why it is separate 
        //from the Set-up - suggest only use when needed 
        private void MineBlockChain()
        { 
            /* Mine blocks, chain should now be completely signed */
            for (int i = 0; i < 5; i++)
            {
                Task T = Task.Factory.StartNew(TestObject[i].Mine);
                while (!T.IsCompleted)
                    System.Threading.Thread.Sleep(1000);
            }
        }

        [TestInitialize]
        public void TestSetup()
        {
            SetBlockChainToInitialState();
            
        }

        [TestMethod]
        public void TestInitializeYieldsBlocks()
        {
            Assert.AreEqual(TestObject.Count, 5);
        }

        [TestMethod]
        public void TestMiningMeansSigning()
        {
            MineBlockChain();
            for (int i = 0; i < 5; i++)
                Assert.IsTrue(TestObject[i].IsSigned);
        }

        [TestMethod]
        public void TestChangeCausesSigningBreak()
        {
            //change nonce in block 0 and test that block 4 is now unsigned

            TestObject[0].Data = "Hi!";
            Assert.IsFalse(TestObject[4].IsSigned);
        }

        [TestMethod]
        public void TestNonces()
        {
            MineBlockChain();
            Assert.AreEqual(TestObject[0].Nonce, 1615);
            Assert.AreEqual(TestObject[1].Nonce, 11008);
            Assert.AreEqual(TestObject[2].Nonce, 32649);
            Assert.AreEqual(TestObject[3].Nonce, 50744);
            Assert.AreEqual(TestObject[4].Nonce, 133232);
        }

        [TestMethod]
        public void TestPreviousHashesBeingUsedCorrectly()
        {
            for (int i = 0; i < 4; i++)
                Assert.AreEqual(TestObject[i].Hash, TestObject[i+1].PreviousHash);
        }
    }
}
