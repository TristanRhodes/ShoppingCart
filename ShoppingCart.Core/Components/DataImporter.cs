using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public class DataImporter : IDataImporter
    {
        private string _dataFile;

        public DataImporter(string dataFile)
        {
            _dataFile = dataFile;
        }

        public List<DataItem> Import()
        {
            var content = File.ReadAllText(_dataFile);
            var items = new List<DataItem>();

            using (var stream = File.OpenRead(_dataFile))
            using (var reader = new StreamReader(stream))
            {
                // Skip header
                if (!reader.EndOfStream)
                    reader.ReadLine();

                while(!reader.EndOfStream)
                {
                    var item = ReadRow(reader);
                    items.Add(item);
                }
            }

            return items;
        }

        private static DataItem ReadRow(StreamReader reader)
        {
            var parts = reader
                .ReadLine()
                .Split(',');

            var item = new DataItem();
            item.Id = Convert.ToInt32(parts[0]);
            item.Name = parts[1];
            item.Description = parts[2];
            item.Stock = Convert.ToInt32(parts[3]);
            item.Price = Convert.ToDecimal(parts[4]);
            return item;
        }
    }
}
