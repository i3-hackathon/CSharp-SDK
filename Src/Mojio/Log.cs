﻿using Mojio.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojio
{
    public enum LogLevels
    {
        /// <summary>Information iteresting to a developer</summary>
        Debug,
        /// <summary>Information interesting to support</summary>
        Info,
        /// <summary>An unexpected situation that the thread/task can recover from</summary>
        Warning,
        /// <summary>An unexpected situation that the thread/task cannot recover from</summary>
        Error,
        /// <summary>An unexpected situation that the app or process cannot recover from and has to exit</summary>
        Fatal
    }

    [JsonConverter(typeof(DiscriminatorConverter<Log>))]   
    public class Log : GuidEntity, IOwner
    {
        public LogLevels Level { get; set; }
        public DateTime Time { get; set; }

        [Observable(typeof(App))]
        public Guid? AppId { get; set; }

        [Observable(typeof(User))]
        public Guid? OwnerId { get; set; }

        public string Message { get; set; }
        public string Environment { get; set; }
        public List<GuidEntity> Entities { get; set; }
        public List<string> Tags { get; set; }

        public Log()
        {
            Level = LogLevels.Info;
            Time = DateTime.UtcNow;
        }

        
        public static Log Create(string message)
        {
            return new Log() { Message = message };
        }

        public static Log Create(Exception ex)
        {
            return new Log().WithException(ex);
        }

        public Log AsWarning()
        {
            Level = LogLevels.Warning;
            return this;
        }

        public Log AsError()
        {
            Level = LogLevels.Error;
            return this;
        }

        public Log AsFatal()
        {
            Level = LogLevels.Fatal;
            return this;
        }

        public Log AsDebug()
        {
            Level = LogLevels.Debug;
            return this;
        }

        public Log WithException(Exception ex)
        {
            if (Level < LogLevels.Warning)
                Level = LogLevels.Warning;

            Line(ex.Message);
            Line(ex.StackTrace);

            return this;
        }

        public Log ForEntity(GuidEntity entity)
        {
            if (Entities == null)
                Entities = new List<GuidEntity>();
            Entities.Add(entity);

            if (OwnerId == null)
            {
                IOwner owned = entity as IOwner;
                if (owned != null)
                    this.OwnerId = owned.OwnerId;
            }

            return this;
        }

        public Log Tag(string tag)
        {
            if (Tags == null)
                Tags = new List<string>();
            Tags.Add(tag);
            return this;
        }

        public Log AtTime(DateTime time)
        {
            Time = time;
            return this;
        }

        public Log Line(string line)
        {
            if (Message == null)
                Message = line;
            else
                Message = '\n' + line;
            return this;
        }

        public Log Token(Token token)
        {
            AppId = token.AppId;
            OwnerId = token.UserId;
            return this;
        }
    }
}
