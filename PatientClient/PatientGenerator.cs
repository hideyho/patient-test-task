using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientClient
{
    public class PatientGenerator
    {
        private readonly List<KeyValuePair<string, List<string>>> _names = new List<KeyValuePair<string, List<string>>>()
        {
            new KeyValuePair<string, List<string>>("Male", new List<string>()
            {
                "Иван", "Андрей", "Дмитрий", "Егор", "Алексей", "Виталий"
            }),
            new KeyValuePair<string, List<string>>("Female", new List<string>
            {
                "Юлия", "Жанна", "Екатерина", "Надежда", "Татьяна", "Ксения"
            }),
            new KeyValuePair<string, List<string>>("Other", new List<string>
            {
                 "Иван", "Андрей", "Дмитрий","Надежда", "Татьяна", "Ксения"
            }),
            new KeyValuePair<string, List<string>>("Unknown", new List<string>
            {
                "Егор", "Алексей", "Виталий","Юлия", "Жанна", "Екатерина",
            })

        };

        private readonly List<string> _families = new List<string>()
        {
            "Иванов", "Сидоров", "Петров", "Георгиев", "Алапенко", "Андреев", "Курочкин"
        };

        private readonly List<string> _patronymics = new List<string>()
        {
            "Иванович", "Алексеевич", "Дмитриевич", "Геннадьевич", "Андреевич", "Артемович"
        };

        public List<PatientModel> GenerateList(int count)
        {
            var list = new List<PatientModel>();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var patientName = _names[random.Next(_names.Count() - 1)];
                var family = _families[random.Next(_families.Count() - 1)];
                var patronymic = _patronymics[random.Next(_patronymics.Count() - 1)];

                list.Add(new PatientModel()
                {
                    Name = new NameModel()
                    {
                        Family = family,
                        Given = new List<string>()
                        {
                            patientName.Value[random.Next(patientName.Value.Count() - 1 )],
                            patronymic
                        },
                        Use = "official"
                    },
                    Active = true,
                    BirthDate = RandomDate(random),
                    Gender = patientName.Key,
                });
            }

            return list;
        }

        private DateTime RandomDate(Random random)
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.Date.AddDays(random.Next(range));
        }

    }
}
