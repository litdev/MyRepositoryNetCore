﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.HttpJob.Client;
using MyNetCore.IServices;
using MyNetCore.Model;

namespace MyNetCore.Services
{
    /// <summary>
    /// Hangfire HttpJob 任务
    /// <code>https://github.com/yuzd/Hangfire.HttpJob/wiki/00.QickStart</code>
    /// </summary>
    [ServiceLifetime(false)]
    public class TaskJobServices : ITaskJobServices
    {
        private static readonly string ServerUrl = AppSettings.Get("HangfireTask:ServerUrl");
        private static readonly string BasicUserName = AppSettings.Get("HangfireTask:BasicUserName");
        private static readonly string BasicPassword = AppSettings.Get("HangfireTask:BasicPassword");
        private static readonly string NoticeMail = AppSettings.Get("HangfireTask:NoticeMail");


        #region 添加任务

        /// <summary>
        /// 添加一个一次性任务，指定多少分钟后执行
        /// </summary>
        /// <param name="jobName">model</param>
        /// <returns>返回jobId，删除时需要</returns>
        public async Task<ApiResult> AddBackgroudJobDelayAsync(AddTaskJobModel model)
        {
            var result = await AddBackgroundJobAsync(new BackgroundJob()
            {
                JobName = model.JobName,
                Url = model.Url,
                Method = model.Method,
                DelayFromMinutes = model.DelayFromMinutes,
            });

            return result.IsSuccess ? ApiResult.OK("OK", result.JobId) : ApiResult.Error(result.ErrMessage);
        }

        /// <summary>
        /// 添加一个一次性任务，指定执行时间
        /// </summary>
        /// <param name="model">任务名称</param>
        /// <returns>返回jobId，删除时需要</returns>
        public async Task<ApiResult> AddBackgroudJobRunAtAsync(AddTaskJobModel model)
        {
            var result = await AddBackgroundJobAsync(new BackgroundJob()
            {
                JobName = model.JobName,
                Url = model.Url,
                Method = model.Method,
                RunAt = model.RunAt,
            });

            return result.IsSuccess ? ApiResult.OK("OK", result.JobId) : ApiResult.Error(result.ErrMessage);

        }

        /// <summary>
        /// 添加一个指定Corn的任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<ApiResult> AddRecurringCornJob(AddTaskJobModel model)
        {
            var result = AddRecurringJob(new RecurringJob()
            {
                JobName = model.JobName,
                Url = model.Url,
                Method = model.Method,
                Cron = model.Corn,
            });

            return Task.FromResult(result.IsSuccess ? ApiResult.OK() : ApiResult.Error(result.ErrMessage));

        }

        #endregion

        #region 删除任务


        /// <summary>
        /// 根据jobid删除一个一次性任务
        /// </summary>
        /// <param name="jobId">添加时返回的JobId</param>
        /// <returns></returns>
        public async Task<ApiResult> RemoveBackgroundJobAsync(string jobId)
        {
            var result = await DeleteBackgroundJobAsync(jobId);

            return result.IsSuccess ? ApiResult.OK() : ApiResult.Error(result.ErrMessage);
        }

        /// <summary>
        /// 根据jobName删除周期任务
        /// </summary>
        /// <param name="jobName">添加时指定的JobName</param>
        /// <returns></returns>
        public async Task<ApiResult> RemoveRecurringJobAsync(string jobName)
        {
            var result = await DeleteRecurringJobAsync(jobName);

            return result.IsSuccess ? ApiResult.OK() : ApiResult.Error(result.ErrMessage);
        }


        #endregion


        #region 简单封装


        /// <summary>
        /// 添加一个一次性任务
        /// </summary>
        /// <param name="job"></param>
        /// <returns>如果返回为空，则执行成功，否则返回异常消息</returns>
        Task<AddBackgroundHangfirJobResult> AddBackgroundJobAsync([NotNull] BackgroundJob job)
        {
            if (job.SendFail || job.SendSuccess) job.Mail = NoticeMail.SplitWithSemicolon().ToList();

            //如果指定的时间小于现在，则置为立即执行
            if (job.RunAt <= DateTime.Now) job.DelayFromMinutes = 0;

            return HangfireJobClient.AddBackgroundJobAsync(ServerUrl, job, new HangfireServerPostOption { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
        }

        /// <summary>
        /// 根据jobid删除一个一次性任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<HangfirJobResult> DeleteBackgroundJobAsync([NotNull] string jobId)
        {
            return HangfireJobClient.RemoveBackgroundJobAsync(ServerUrl, jobId, new HangfireServerPostOption() { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
        }

        /// <summary>
        ///  添加一个指定Corn的任务
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        HangfirJobResult AddRecurringJob([NotNull] RecurringJob job)
        {
            if (job.SendFail || job.SendSuccess) job.Mail = NoticeMail.SplitWithSemicolon().ToList();

            return HangfireJobClient.AddRecurringJob(ServerUrl, job, new HangfireServerPostOption { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
        }

        /// <summary>
        /// 根据jobName删除周期任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        Task<HangfirJobResult> DeleteRecurringJobAsync([NotNull] string jobName)
        {
            return HangfireJobClient.RemoveRecurringJobAsync(ServerUrl, jobName, new HangfireServerPostOption() { BasicUserName = BasicUserName, BasicPassword = BasicPassword });
        }

        #endregion
    }
}
