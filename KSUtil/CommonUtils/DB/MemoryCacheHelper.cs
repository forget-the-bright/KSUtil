using System;
using System.Runtime.Caching;
namespace KSUtil.CommonUtils.DB
{
    /// <summary>
    /// 提供对 MemoryCache 的操作方法。
    /// </summary>
    public static class MemoryCacheHelper
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        /// <summary>
        /// 将对象添加到 MemoryCache 中，并设置绝对过期时间。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public static void Add(string key, object value, DateTimeOffset absoluteExpiration)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration
            };
            Cache.Add(key, value, policy);
        }

        /// <summary>
        /// 将对象添加到 MemoryCache 中，并设置滑动过期时间。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">要缓存的对象。</param>
        /// <param name="slidingExpiration">滑动过期时间。</param>
        public static void AddWithSlidingExpiration(string key, object value, TimeSpan slidingExpiration)
        {
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = slidingExpiration
            };
            Cache.Add(key, value, policy);
        }

        /// <summary>
        /// 将对象添加到 MemoryCache 中，并设置缓存项的依赖项。
        /// </summary>
        /// <param name="key">缓存键。</param>
        /// <param name="value">要缓存的对象。</param>
        /// <param name="dependencies">缓存项的依赖项。</param>
        public static void AddWithDependencies(string key, object value, CacheEntryChangeMonitor dependencies)
        {
            var policy = new CacheItemPolicy
            {
                ChangeMonitors = { dependencies }
            };
            Cache.Add(key, value, policy);
        }

        /// <summary>
        /// 获取 MemoryCache 中指定键的对象。
        /// </summary>
        /// <typeparam name="T">要获取的对象的类型。</typeparam>
        /// <param name="key">缓存键。</param>
        /// <returns>MemoryCache 中指定键的对象，如果不存在则为默认值。</returns>
        public static T Get<T>(string key)
        {
            return Cache.Contains(key) ? (T)Cache.Get(key) : default;
        }

        /// <summary>
        /// 从 MemoryCache 中移除指定键的对象。
        /// </summary>
        /// <param name="key">缓存键。</param>
        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 清除 MemoryCache 中的所有对象。
        /// </summary>
        public static void Clear()
        {
            Cache.Trim(100); // 清除所有缓存项
        }

        /// <summary>
        /// 检查指定键是否存在于缓存中。
        /// </summary>
        /// <param name="key">要检查的缓存键。</param>
        /// <returns>如果缓存中存在指定的键，则为 true；否则为 false。</returns>
        public static bool Contains(string key)
        {
            return Cache.Contains(key);
        }
    }
}
