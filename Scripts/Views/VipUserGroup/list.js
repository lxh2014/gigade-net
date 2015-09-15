var pageSize = 25;
Ext.define('gigade.VipUserGroup', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "group_id", type: "int" },
      { name: "group_name", type: "string" },
      { name: "domain", type: "string" },
         { name: "tax_id", type: "string" },
    { name: "group_code", type: "string" },
    { name: "group_capital", type: "int" },
    { name: "group_emp_number", type: "int" },
        { name: "group_emp_age", type: "string" },
    { name: "group_emp_gender", type: "int" },
    { name: "group_benefit_type", type: "string" },
    { name: "group_benefit_desc", type: "string" },
    { name: "group_subsidiary", type: "int" },
     { name: "group_hq_name", type: "string" },
    { name: "group_hq_code", type: "string" },
    { name: "group_committe_name", type: "string" },
    { name: "group_committe_code", type: "string" },
{ name: "group_committe_chairman", type: "string" },
    { name: "group_committe_phone", type: "string" },
{ name: "group_committe_mail", type: "string" },
    { name: "group_committe_promotion", type: "string" },
      { name: "group_committe_desc", type: "string" },
          { name: "image_name", type: "string" },
    { name: "gift_bonus", type: "int" },
{ name: "createdate", type: "int" },
    { name: "group_category", type: "int" },
{ name: "bonus_rate", type: "int" },
    { name: "bonus_expire_day", type: "int" },
      { name: "eng_name", type: "string" },
          { name: "check_iden", type: "int" },
      { name: "site_id", type: "int" },
      { name: "group_status", type: "int" },
    { name: "k_user", type: "int" },
    { name: "k_date", type: "string" },
    { name: "m_user", type: "int" },
    { name: "m_date", type: "string" },
        { name: "create_user", type: "string" },
       { name: "update_user", type: "string" },
      { name: "file_name", type: "string" },
            
    ]
});
var VipUserGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipUserGroup',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/VipUserGroup/GetVipUserGList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define('gigade.edit_VipUserGroup', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "group_id", type: "int" },
      { name: "group_name", type: "string" },
      { name: "domain", type: "string" },
         { name: "tax_id", type: "string" },
    { name: "group_code", type: "string" },
    { name: "group_capital", type: "int" },
    { name: "group_emp_number", type: "int" },
        { name: "group_emp_age", type: "string" },
    { name: "group_emp_gender", type: "int" },
    { name: "group_benefit_type", type: "string" },
    { name: "group_benefit_desc", type: "string" },
    { name: "group_subsidiary", type: "int" },
     { name: "group_hq_name", type: "string" },
    { name: "group_hq_code", type: "string" },
    { name: "group_committe_name", type: "string" },
    { name: "group_committe_code", type: "string" },
{ name: "group_committe_chairman", type: "string" },
    { name: "group_committe_phone", type: "string" },
{ name: "group_committe_mail", type: "string" },
    { name: "group_committe_promotion", type: "string" },
      { name: "group_committe_desc", type: "string" },
          { name: "image_name", type: "string" },
    { name: "gift_bonus", type: "int" },
{ name: "createdate", type: "int" },
    { name: "group_category", type: "int" },
{ name: "bonus_rate", type: "int" },
    { name: "bonus_expire_day", type: "int" },
      { name: "eng_name", type: "string" },
          { name: "check_iden", type: "int" },
      { name: "site_id", type: "int" },
      { name: "group_status", type: "int" },
    { name: "k_user", type: "int" },
    { name: "k_date", type: "string" },
    { name: "m_user", type: "int" },
    { name: "m_date", type: "string" },
        { name: "create_user", type: "string" },
            { name: "update_user", type: "string" },
                  { name: "file_name", type: "string" },
    ]
});
var edit_VipUserGroupStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.edit_VipUserGroup',
    proxy: {
        type: 'ajax',
        url: '/VipUserGroup/GetVipUserGList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }

});


VipUserGroupStore.on('beforeload', function () {
    Ext.apply(VipUserGroupStore.proxy.extraParams,
        {
            isSecret: true,
        });
});
edit_VipUserGroupStore.on('beforeload', function () {
    Ext.apply(VipUserGroupStore.proxy.extraParams,
        {
            isSecret: false,
        });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("VipUserGroup").getSelectionModel().getSelection();
            Ext.getCmp("VipUserGroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {
    var VipUserGroup = Ext.create('Ext.grid.Panel', {
        id: 'VipUserGroup',
        store: VipUserGroupStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.1,
        columns: [
              { header: "公司編號", dataIndex: 'group_id', width: 150, align: 'center' },
        { header: "中文名稱", dataIndex: 'group_name', width: 150, align: 'center' },
        { header: "公司統編", dataIndex: 'group_code', width: 150, align: 'center' },
         {
             header: "福委聯絡人", width: 100, align: 'center',id: 'reportForm',
             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                 return "<a href='javascript:void(0)' onclick='SecretLogin(" + record.data.group_id + ")'><img src='../../../Content/img/icons/application_view_list.png' /></a>"
             }
         },
        //{
        //    header: "福委姓名", dataIndex: 'group_committe_chairman', width: 120, align: 'center',
        //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
        //        return "<span onclick='SecretLogin(" + record.data.group_id + ")'  >" + value + "</span>";
        //    }
        //},
        //{
        //    header: "福委電話", dataIndex: 'group_committe_phone', width: 120, align: 'center',
        //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
        //        return "<span onclick='SecretLogin(" + record.data.group_id + ")'  >" + value + "</span>";
        //    }
        //},
        //{
        //    header: "福委mail", dataIndex: 'group_committe_mail', width: 120, align: 'center',
        //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
        //        return "<span onclick='SecretLogin(" + record.data.group_id + ")'  >" + value + "</span>";
        //    }
        //},
        { header: "創建人員", dataIndex: 'create_user', width: 120, align: 'center' },
        { header: "創建日期", dataIndex: 'k_date', width: 160, align: 'center' },
        { header: "異動人員", dataIndex: 'update_user', width: 120, align: 'center' },
        { header: "異動日期", dataIndex: 'm_date', width: 160, align: 'center' },
         {
             header: "啟用", dataIndex: 'group_status', width: 120, align: 'center',
             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                 if (value == 1) {
                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.group_id + ")'><img hidValue='0' id='img" + record.data.group_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                 } else {
                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.group_id + ")'><img hidValue='1' id='img" + record.data.group_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                 }
             }
         },

        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
        { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
        '->',
        {
            xtype: 'textfield',
            fieldLabel: '公司統編/中文名稱',
            labelWidth:'120',
            id: 'tax_name',
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Query();
                    }
                }
            }
        },
        {
            xtype: 'button',
            text: '查詢',
            handler:Query,
        },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VipUserGroupStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [VipUserGroup],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                VipUserGroup.width = document.documentElement.clientWidth;

                this.doLayout();
            }
        }
    });
    ToolAuthority();
});
function Query() {
    if (Ext.getCmp('tax_name').getValue() == "")
    {
        Ext.Msg.alert("提示信息", "請輸入查詢條件");
        return;
    }
    Ext.getCmp("VipUserGroup").store.loadPage(1, {
        params: {
            tax_name: Ext.getCmp('tax_name').getValue(),
        }
    });

}

//*********新增********//
onAddClick = function () {
    editFunction(null);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("VipUserGroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        var secret_type = "18";//參數表中的企業會員管理
        var url = "/VipUserGroup/GetVipUserGList/Edit";
        var ralated_id = row[0].data.group_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
            }
            else {
                editFunction(ralated_id);
            }
        }
     
    }
}
//******更改狀態******//
function UpdateActive(row_id) {
    var activeValue = $("#img" + row_id).attr("hidValue");
    $.ajax({
        url: "/VipUserGroup/UpVUGStatus",
        data: {
            "group_id": row_id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "no") {
                Ext.Msg.alert("提示信息","此企業用戶正在參加活動，不可停用！");
            }
            else {
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                    VipUserGroupStore.load();
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                    VipUserGroupStore.load();
                }
            }

        },
        error: function (msg) {
            Ext.Msg.alert("提示信息", "操作失敗");
            VipUserGroupStore.load();
        }
    });
}

function SecretLogin(rid) {//secretcopy
    var secret_type = "18";//參數表中的"企業會員管理"
    var url = "/VipUserGroup/Index ";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}