﻿using System;

namespace Blogging.API.Dtos
{
    public class BlogDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Heading { get; set; }
        public string Footer { get; set; }
        public string Uri { get; set; }
        public string Path { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DisabledAt { get; set; }
    }
}