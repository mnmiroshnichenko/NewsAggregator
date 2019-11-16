﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAggregator.Models
{
    public class NewsRepository : INewsRepository
    {
        MongoDbContext db = new MongoDbContext();

        public async Task<NewsEntry> GetNewsEntry(string id)
        {
            try
            {
                FilterDefinition<NewsEntry> filter = Builders<NewsEntry>.Filter.Eq("Id", id);
                foreach (var news in db.News)
                {
                    var entry = news.Find(filter);
                    if (entry.Any())
                        return await entry.FirstOrDefaultAsync();
                }
                throw new Exception("Invalid id");
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<NewsEntry>> GetAllNews()
        {
            try
            {
                var allNews = db.News;

                //some smart logic here e.g. news comparation
                //actually the result of some smart function should be here
                var selectedNews = await MergeNewsAsync(allNews);
                
                var sortedNews = selectedNews.
                    OrderByDescending(news => news.TimeSourcePublished);
                return sortedNews;
            }
            catch
            {
                throw;
            }
        }

        private async Task<List<NewsEntry>> MergeNewsAsync(List<IMongoCollection<NewsEntry>> allNews)
        {
            var res = new List<NewsEntry>();

            foreach(var news in allNews)
            {
                res.InsertRange(res.Count,
                    await news.Find(_ => true).ToListAsync());
            }
            return res;
        }
    }
}