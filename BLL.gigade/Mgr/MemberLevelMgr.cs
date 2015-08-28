using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class MemberLevelMgr
    {
        private MemberLevelDao _memberLevel;

        public MemberLevelMgr(string connectionString)
        {
            _memberLevel = new MemberLevelDao(connectionString);
        }

        public List<MemberLevelQuery> MemberLevelList(MemberLevelQuery query, out int totalCount)
        {
            try
            {
                return _memberLevel.MemberLevelList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberLevelMgr-->MemberLevelList-->" + ex.Message, ex);
            }
        }

        public int UpdateActive(MemberLevelQuery query)
        {
            try
            {
                return _memberLevel.UpdateActive(query);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberLevelMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }

        public int SaveMemberLevel(MemberLevelQuery query)
        {
            try
            {
                return _memberLevel.SaveMemberLevel(query);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberLevelMgr-->SaveMemberLevel-->" + ex.Message, ex);
            }
        }

        public int MaxMLSeq()
        {
            try
            {
                return _memberLevel.MaxMLSeq();
            }
            catch (Exception ex)
            {
                throw new Exception("MemberLevelMgr-->MaxMLSeq-->" + ex.Message, ex);
            }
        }

        public bool DistinctCode(MemberLevelQuery query)
        {
            try
            {
                return _memberLevel.DistinctCode(query);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberLevelMgr-->DistinctCode-->" + ex.Message, ex);
            }
        }

        public bool DistinctSeq(MemberLevelQuery query)
        {
            try
            {
                return _memberLevel.DistinctSeq(query);
            }
            catch (Exception ex)
            {
                throw new Exception("MemberLevelMgr-->DistinctSeq-->" + ex.Message, ex);
            }
        }
        public DataTable GetLevel()
       {
         try
         {
          return _memberLevel.GetLevel();
         }
         catch (Exception ex)
         {
             throw new Exception("MemberLevelMgr-->GetLevel-->" + ex.Message, ex);
         }
     }
    }
}
