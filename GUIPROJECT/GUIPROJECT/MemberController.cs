﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace GUIPROJECT
{
    class MemberController
    {
        DBManager dbMan;
        public MemberController()
        {
            dbMan = new DBManager();
        }


        public void TerminateConnection()
        {
            dbMan.CloseConnection();
        }
        public int Addevent(string name,string place,string date, int fees, int memberid)
        {
            string query = "Insert into Events values('"+name+"','"+place+"','"+date+"',null,"+fees+","+memberid+")";
            return dbMan.ExecuteNonQuery(query);
        }
    }
}
