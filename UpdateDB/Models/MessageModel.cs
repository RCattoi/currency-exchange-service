using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpdateDB.Models
{
    public class MessageModel(string action, CurrencyListModel message)
  {
    public string Action { get; set; } = action;
    public CurrencyListModel Message { get; set; } = message;
  }
}