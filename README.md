# SimpleBC
Primarily intended for teaching purposes - a visual demo of how a blockchain works

## Simple Block Chain Demo
 A visual demonstration of a block chain, suggested by Anders Brownworth's excellent 
            YouTube 'Blockchain 101 - A Visual Demo' which can be found [here](https://www.youtube.com/watch?v=_160oMzblY8&amp;t=11s)

A block may be 'signed'. This means that the hash satisfies some condition. For this demo the                 (simple) condition is that the Hash should start with four zeros. Visually, a signed block will have a green background and an unsigned one a red background. 

The hash of a block is calculated from four things
+ its ID (immutable, 0..4 in this demo)
+ its Nonce (an editable arbitrary positive integer)
+ its Data payload (editable, may be empty)
+ the hash of the previous block (apart from block ID=0 which gets a constant as its previous hash)

A block may be 'mined' by pressing its Mine button. This basically means find a nonce (for given ID, Data, and Previous Hash) that will cause the new hash to satisfy the signing condition. Given how hashes work, such a nonce is unpredictable. Nonces could perfectly well be tried randomly, but here we begin at 0 and try each integer in turn until we find one that works. This has the advantage that *ceteris paribus* you the same nonce on repeated trials.

Mining can take a long time - nonces  >100k are not unusual. More stringent hashing algorithms than SHA1 will cause it to take longer still, the nonce expectation depends only on the signing condition but calculating the new hash will take longer and this has to be done for every trial nonce.

## TL;DR

The key thing to note here is how, changing any block causes a cascade of changes down the chain;  the edited blocks and all blocks following it become unsigned. The chain *could*                be re-mined - one block at a time - but this is expensive: unfeasibly  expensive with a sufficiently stringent signing condition.

## Configuration
+ The hashing algorithm used can be changed via the  initialization in the MainWindow constructor, see the comment there.
+  The signing condition can be changed by altering signKey in the Block class in the Block.cs file. A block is signed if its hash **starts** with signKey.

An enhancement would be to support passing a RegEx here rather than a simple string.
Setting a signKey that can never be matched (eg longer than the hash, containing characters other than [0-9a-f]) will cause mining to break.  You were warned.
