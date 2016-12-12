using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoansByUser
{
    public string NameLean { get; set; }
    public string LeanLean { get; set; }
    public string BackTillLean { get; set; }
    public bool ToBackLean { get; set; }
    public bool ReservedLean { get; set; }

    public LoansByUser(string name, string leanerName, DateTime? backTill, bool toBack, bool isRes)
    {
        NameLean = name;
        LeanLean = leanerName;
        BackTillLean = backTill.ToString();
        ToBackLean = toBack;
        ReservedLean = isRes;
    }

}