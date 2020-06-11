﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XueLeMeBackend.Models
{
    public class AdditionalImage
    {
        public int Id { get; set; }
        public BinaryFile ImageFile { get; set; }
        public int ImageFileId { get; set; }
        public TextAndImageContent TextAndImageContent { get; set; }
        public int TextAndImageContentId { get; set; }
    }
}
