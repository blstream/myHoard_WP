using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models
{
    public class Collection : BaseEntity
    {

        static private string TagSeparator = "<;>";

        private string name;
        private string description;
        private string thumbnail;
        private string tags;
        private int itemsNumber;
        private DateTime createdDate;
        private DateTime modifiedDate;
        private bool isPrivate;

        public Collection()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }


        
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                ModifiedDate = DateTime.Now;
                Desync();
            }
        }

        
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                ModifiedDate = DateTime.Now;
                Desync();
            }
        }

        
        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
                ModifiedDate = DateTime.Now;
                Desync();
            }
        }

        
        public bool IsPrivate
        {
            get { return isPrivate; }
            set { isPrivate = value; }
        }

        
        public string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                ModifiedDate = DateTime.Now;
                Desync();
            }
        }

        
        public int ItemsNumber
        {
            get { return itemsNumber; }
            set
            {
                itemsNumber = value;
                ModifiedDate = DateTime.Now;
                Desync();
            }
        }

        
        public DateTime CreatedDate
        {
            get { return createdDate; }
            set
            {
                createdDate = value;
            }
        }

        
        public DateTime ModifiedDate
        {
            get { return modifiedDate; }
            set
            {
                modifiedDate = value;
            }
        }

        [Ignore]
        public List<string> TagList
        {
            get { return getTagList(); }
            set
            {
                setTagList(value);
            }
        }

        private List<string> getTagList()
        {
            List<String> tagList = new List<string>();
            if (!String.IsNullOrEmpty(Tags))
                tagList = Tags.Split(new string[] { TagSeparator }, StringSplitOptions.None).ToList<string>();

            return tagList;
        }

        private void setTagList(ICollection<string> tagList)
        {
            Tags = "";
            foreach (string tag in tagList)
            {
                Tags += tag + TagSeparator;
            }
            if (Tags.Length > TagSeparator.Length)
                Tags = Tags.Remove(Tags.Length - TagSeparator.Length);
        }
    }
}