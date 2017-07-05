﻿using System;

namespace Fiver.Azure.NoSql.Client.Models.Movies
{
    public class MovieOutputModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Summary { get; set; }
        public DateTime LastReadAt { get; set; }
    }
}
