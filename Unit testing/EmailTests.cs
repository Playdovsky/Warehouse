using System.Text.RegularExpressions;

namespace Unit_testing
{
    public class EmailTests
    {
        [Fact]
        public void GoodEmail1()
        {
            string email = "matthew.stick@gmail.com";

            Assert.True(Check(email));
        }

        [Fact]
        public void GoodEmail2()
        {
            string email = "CarlosFontanciones645@o2.pl";

            Assert.True(Check(email));
        }

        [Fact]
        public void GoodEmail3()
        {
            string email = "Daw_Kun@wp.pl";

            Assert.True(Check(email));
        }

        [Fact]
        public void BadEmail1()
        {
            string email = "VicPeczkin_onet.eu";

            Assert.False(Check(email));
        }

        [Fact]
        public void BadEmail2()
        {
            string email = "Piosm@gmail,com";

            Assert.False(Check(email));
        }


        private static bool Check(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, pattern);
        }
    }
}
