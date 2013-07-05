using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using searchpd.Models;
using Version = Lucene.Net.Util.Version;

namespace searchpd.Search
{
    public abstract class LuceneSearcher
    {
        protected const Version LuceneVersion = Version.LUCENE_30;

        // This object is a singleton, so should be able to store things safely in a property.
        protected IndexSearcher _searcher;

        /// <summary>
        /// Ensures that the data has been loaded in the Lucene index 
        /// 
        /// SIDE EFFECT
        /// This method sets property _searcher
        /// </summary>
        /// <param name="absoluteLucenePath">
        /// Absolute path of the directory where the Lucene files get stored.
        /// </param>
        /// <param name="onlyIfNotExists">
        /// If true, the index will only be created if there is no index at all (that is, no Lucene directory).
        /// If false, this will always create a new index.
        /// </param>
        /// <param name="loadLuceneIndex">
        /// Actually loads data into the Lucene index
        /// </param>
        protected void LoadStore(string absoluteLucenePath, bool onlyIfNotExists, Action<SimpleFSDirectory> loadLuceneIndex)
        {
            var luceneDir = new DirectoryInfo(absoluteLucenePath);
            bool luceneDirExists = luceneDir.Exists;

            if (!luceneDirExists)
            {
                luceneDir.Create();
                luceneDir.Refresh();
            }

            var directory = new SimpleFSDirectory(luceneDir);

            if (onlyIfNotExists && luceneDirExists)
            {
                // Make sure to always create a searcher.
                // There must actually be a valid Lucene index in the directory, otherwise this will throw an exception.
                _searcher = new IndexSearcher(directory, true);

                return;
            }

            loadLuceneIndex(directory);

            // Now that the index has been updated, need to open a new searcher.
            // Existing searcher will still be reading the previous index.

            _searcher = new IndexSearcher(directory, true);
        }

    }
}
