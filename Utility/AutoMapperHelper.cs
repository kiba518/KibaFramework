using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Data;
using System.Collections;
using System.Threading;
using System.Collections.Concurrent;

namespace Utility
{
    /// <summary>
    /// AutoMapper扩展帮助类
    /// </summary>
    public static class AutoMapperHelper
    {
        /// <summary>
        ///  类型映射
        /// </summary>
        public static T MapTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            Mapper.CreateMap(obj.GetType(), typeof(T));
            return Mapper.Map<T>(obj);
        } 
        
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            try
            {
                foreach (var first in source)
                {
                    var type = first.GetType();
                    Mapper.CreateMap(type, typeof(TDestination));
                    break;
                }
                return Mapper.Map<List<TDestination>>(source);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList_Task<TDestination>(this IEnumerable source)
        {
            foreach (var first in source)
            {
                var type = first.GetType();
                Mapper.CreateMap(type, typeof(TDestination));
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            //IEnumerable<T> 类型需要创建元素的映射 
            Mapper.CreateMap<TSource, TDestination>(); 
            return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 类型映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source">源</param>
        /// <param name="destination">目标</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            Mapper.CreateMap<TSource, TDestination>();
            return Mapper.Map(source, destination);
        }
     
        /// <summary>
        /// DataReader映射
        /// </summary>
        public static IEnumerable<T> DataReaderMapTo<T>(this IDataReader reader)
        {
            Mapper.Reset();
            Mapper.CreateMap<IDataReader, IEnumerable<T>>();
            return Mapper.Map<IDataReader, IEnumerable<T>>(reader);
        }
        /// <summary>
        /// 无排序 
        /// </summary> 
        public static List<TDestination> MapToListParallel<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            BlockingCollection<TDestination> result = new BlockingCollection<TDestination>();
            Mapper.CreateMap<TSource, TDestination>();
            CancellationTokenSource cts = new CancellationTokenSource();
            ParallelOptions pOption = new ParallelOptions() { CancellationToken = cts.Token };
            pOption.MaxDegreeOfParallelism = 50;
            BlockingCollection<int> excuteCount = new BlockingCollection<int>();
            Parallel.ForEach(source, pOption, item =>
            { 
                TDestination destination = System.Activator.CreateInstance<TDestination>(); ;
                Mapper.Map(item, destination); 
                result.Add(destination);
                excuteCount.Add(1);
            });

            while (true)
            {
                if (source.Count() == excuteCount.Count())
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            return result.ToList();
        }
         
    }

}
