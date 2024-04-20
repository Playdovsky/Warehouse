namespace Unit_testing
{
    public class PasswordTests
    {
        [Fact]
        public void GoodPassword1()
        {
            string password = "Pa$$w0rd123";

            Assert.True(Check(password));
        }

        [Fact]
        public void GoodPassword2()
        {
            string password = "4#Super#4";

            Assert.True(Check(password));
        }

        [Fact]
        public void GoodPassword3()
        {
            string password = "_City17_";

            Assert.True(Check(password));
        }

        [Fact]
        public void BadPassword1()
        {
            string password = "123";

            Assert.False(Check(password));
        }

        [Fact]
        public void BadPassword2()
        {
            string password = "Qwerty123";

            Assert.False(Check(password));
        }

        [Fact]
        public void BadPassword3()
        {
            string password = "JHDxgfFGHETDFfGVXkj1EZ63JGBNR$EDSVsdgB87CTYFH$%e@GHJF%6&6DS@NVCIP3&*@G@!#DGO345^FH1CN#8xc528KBN%@FDSGBN>777^&M>3#CXBJ%YhgzD#";

            Assert.False(Check(password));
        }

        [Fact]
        public void BadPassword4()
        {
            string password = "Mix2$";

            Assert.False(Check(password));
        }

        [Fact]
        public void BadPassword5()
        {
            string password = "KHVC46$$$76HG";

            Assert.False(Check(password));
        }

        [Fact]
        public void BadPassword6()
        {
            string password = "^k43uh%g";

            Assert.False(Check(password));
        }

        [Fact]
        public void BadPassword7()
        {
            string password = "Welcome_back";

            Assert.False(Check(password));
        }

        public bool Check(string password)
        {
            if (password.Length < 8 || password.Length > 15)
            {
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            string specialCharacters = "-_!*#$&";
            if (!password.Any(c => specialCharacters.Contains(c)))
            {
                return false;
            }

            return true;
        }
    }
}
