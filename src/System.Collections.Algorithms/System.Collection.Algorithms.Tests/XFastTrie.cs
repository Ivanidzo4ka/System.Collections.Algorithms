using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;

namespace System.Collections.Algorithms.Tests
{
    public class XFastTrieTests
    {
        [Fact]
        public void Do()
        {
            var trie = new XFastTrie();
            Assert.True(trie.Add(1));
            Assert.True(trie.Add(byte.MaxValue));
            Assert.True(trie.Add(byte.MinValue));
            Assert.True(trie.Add(7));
            Assert.True(trie.Add(5));
            Assert.True(trie.Add(4));
            Assert.False(trie.Add(4));
            Assert.False(trie.Find(6));
            Assert.True(trie.Find(byte.MinValue));
            Assert.True(trie.Find(1));
            Assert.True(trie.Remove(7));
            Assert.False(trie.Remove(8));
            trie.Remove(4);
            trie.Remove(5);

            Assert.True(trie.Remove(byte.MaxValue));
            Assert.True(trie.Remove(1));
            Assert.True(trie.Remove(byte.MinValue));
        }
    }
}
