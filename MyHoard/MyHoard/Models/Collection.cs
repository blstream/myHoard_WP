using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyHoard.Models
{
    public class Collection : BaseEntity
    {

        public static string TagSeparator = " #";

        private string name = string.Empty;
        private string description = string.Empty;
        private string tags = string.Empty;
        private int itemsNumber;
        private DateTime createdDate;
        private DateTime modifiedDate;
        private bool isPrivate;
        private ImageSource thumbnail;

        public Collection()
        {
            CreatedDate = DateTime.Now;
        }


        
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                IsSynced = false;
                NotifyOfPropertyChange(() => Name);
            }
        }

        
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                IsSynced = false;
                NotifyOfPropertyChange(() => Description);
            }
        }

        [Ignore]
        public ImageSource Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
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
                IsSynced = false;
                NotifyOfPropertyChange(() => Tags);
            }
        }

        
        public int ItemsNumber
        {
            get { return itemsNumber; }
            set
            {
                itemsNumber = value;
                IsSynced = false;
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
            List<string> tagList = new List<string>();
            if (!string.IsNullOrEmpty(Tags))
                tagList = Tags.Split(new string[] { TagSeparator }, StringSplitOptions.None).ToList<string>();
            return tagList.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }

        

        private void setTagList(ICollection<string> tagList)
        {
            
            Tags = "";
            foreach (string tag in tagList)
            {
                Tags += TagSeparator + tag;
            }
            
        }
    }
}