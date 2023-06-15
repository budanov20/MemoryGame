using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public long RoleID { get; set; }
        public long GenderID { get; set; }
        public long EducationLevelID { get; set; }
        public long EducationTypeID { get; set; }
        public int SizeScore { get; set; }
        public int ColorScore { get; set; }
        public int OneColorScore { get; set; }
        public int ColorSizeScore { get; set; }
        public int TotalScore { get { return SizeScore+ColorScore+OneColorScore+ColorSizeScore; } }
        public User()
        {
            SizeScore = 0;
            ColorScore = 0;
            OneColorScore = 0;
            ColorSizeScore = 0;
        }

        public User(long id, string n, string e, string p, int a, int genderID, int roleID, int educationLevelID, int educationTypeID)
        {
            Id = id;
            Name = n;
            Email = e;
            Password = p;
            Age = a;
            GenderID = genderID;
            RoleID = roleID;
            EducationLevelID = educationLevelID;
            EducationTypeID = educationTypeID;
            SizeScore = 0;
            ColorScore = 0;
            OneColorScore = 0;
            ColorSizeScore = 0;
        }

        public User(long id, string n, string e, string p, int a, int genderID, int roleID, int educationLevelID, int educationTypeID, int ss, int cs, int ocs, int css)
        {
            Id = id;
            Name = n;
            Email = e;
            Password = p;
            Age = a;
            GenderID = genderID;
            RoleID = roleID;
            EducationLevelID = educationLevelID;
            EducationTypeID = educationTypeID;
            SizeScore = ss;
            ColorScore = cs;
            OneColorScore = ocs;
            ColorSizeScore = css;
        }
    }

}
