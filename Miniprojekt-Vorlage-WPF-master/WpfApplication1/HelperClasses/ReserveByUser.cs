using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ReserveByUser
{
    public string resCustomer { get; set; }
    public string NameRes { get; set; }
    public string WaitSizeRes { get; set; }
    public bool LeanRes { get; set; }

    public ReserveByUser(string customer, string name, int resSize, bool isRes)
    {
        resCustomer = customer;
        NameRes = name;
        WaitSizeRes = resSize.ToString();
        LeanRes = isRes;
    }
}