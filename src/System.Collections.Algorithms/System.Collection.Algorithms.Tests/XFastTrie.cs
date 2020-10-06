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
            Assert.True(trie.Add(uint.MaxValue));
            Assert.True(trie.Add(uint.MinValue));
            Assert.True(trie.Add(124454));
            Assert.True(trie.Add(124453));
            Assert.False(trie.Add(124453));
            Assert.False(trie.Find(123123));
            Assert.True(trie.Find(uint.MinValue));
            Assert.True(trie.Find(1));
            Assert.True(trie.Remove(124454));
            Assert.False(trie.Remove(124454));
            Assert.True(trie.Remove(uint.MaxValue));
            Assert.True(trie.Remove(1));
            Assert.True(trie.Remove(uint.MinValue));
        }
    }
}
