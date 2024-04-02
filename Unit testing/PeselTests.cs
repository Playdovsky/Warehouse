namespace Unit_testing
{
    public class PeselTests
    {
        [Fact]
        public void GoodPesel1()
        {
            string pesel = "02321513432";
            DateTime birthDate = new(2002, 12, 15);
            string gender = "Male";

            Assert.True(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void GoodPesel2()
        {
            string pesel = "99042496229";
            DateTime birthDate = new(1999, 04, 24);
            string gender = "Female";

            Assert.True(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void BadPesel1()
        {
            string pesel = "95050782211";
            DateTime birthDate = new(1995, 05, 07);
            string gender = "Female";

            Assert.False(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void BadPesel2()
        {
            string pesel = "03211218570";
            DateTime birthDate = new(2003, 01, 12);
            string gender = "Male";

            Assert.False(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void BadPesel3()
        {
            string pesel = "91082998614";
            DateTime birthDate = new(1991, 09, 29);
            string gender = "Male";

            Assert.False(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void BadPesel4()
        {
            string pesel = "83071584832";
            DateTime birthDate = new(1983, 07, 16);
            string gender = "Male";

            Assert.False(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void BadPesel5()
        {
            string pesel = "650818822I82";
            DateTime birthDate = new(1965, 08, 18);
            string gender = "Female";

            Assert.False(Check(pesel, birthDate, gender));
        }

        [Fact]
        public void BadPesel6()
        {
            string pesel = "780320869870";
            DateTime birthDate = new(1978, 03, 20);
            string gender = "Female";

            Assert.False(Check(pesel, birthDate, gender));
        }

        private static bool Check(string pesel, DateTime birthDate, string gender)
        {
            if (pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                return false;
            }

            int year = int.Parse(pesel[..2]);
            int month = int.Parse(pesel.Substring(2, 2));
            int day = int.Parse(pesel.Substring(4, 2));

            int birthYear = 1900 + year;
            if (month > 12)
            {
                birthYear += 100;
                month -= 20;
            }

            if (birthDate.Year != birthYear || birthDate.Month != month || birthDate.Day != day)
            {
                return false;
            }

            int genderDigit = int.Parse(pesel.Substring(9, 1));
            bool isMale = genderDigit % 2 == 1;

            if ((gender == "Male" && !isMale) || (gender == "Female" && isMale))
            {
                return false;
            }

            int[] weights = [1, 3, 7, 9, 1, 3, 7, 9, 1, 3];
            int sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(pesel[i].ToString()) * weights[i];
            }

            int controlNumber = (10 - (sum % 10)) % 10;

            return controlNumber == int.Parse(pesel[10].ToString());
        }
    }
}
