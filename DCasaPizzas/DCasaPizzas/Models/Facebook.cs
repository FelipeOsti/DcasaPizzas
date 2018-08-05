using System;
using System.Collections.Generic;
using System.Text;

namespace DCasaPizzas.Models
{
    public class Facebook
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Picture Picture { get; set; }
    }
    public class Picture
    {
        public PictureData Data { get; set; }
    }
    public class PictureData
    {
        public bool Is_Silhouette { get; set; }
        public string Url { get; set; }
    }
}
