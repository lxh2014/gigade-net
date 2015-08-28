Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "v_id", type: "int" },
        { name: "vuser_email", type: "string" },
        { name: "status", type: "int" },
        { name: "emp_id", type: "string" },
        { name: "screatedate", type: "string" },


        { name: "user_id", type: "string" },
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_email", type: "string" },
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
    //        {name: "user_birthday_year", type: "int" }, //年
    //        {name: "user_birthday_month", type: "int" }, //月
    //        {name: "user_birthday_day", type: "int" }, //日
        { name: "birthday", type: "string" },
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //行動電話
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_phone", type: "string" }, //聯絡電話
        { name: "reg_date", type: "string" }, //註冊日期
       
        
        { name: "user_source", type: "string" },
    //        {name: "user_type",type:"string"},
        { name: "mytype", type: "string" }, //用戶類別
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告
        { name: "adm_note", type: "string"},//管理員備註
    { name: "paper_invoice", type: "bool" }

      ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        //url:controller/方法名
        url: '/Member/GetVipUserList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            //            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#remove').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#auth').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#callid').setDisabled(selections.length == 0);
        }
    }
});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有會員資料", "value": "0" },
        { "txt": "電子信箱", "value": "1" }

    ]
});

FaresStore.on('beforeload', function () {
    Ext.apply(FaresStore.proxy.extraParams,
        {
            serchs: Ext.getCmp('serchs').getValue(),
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            groupid: document.getElementById("groupid").value
        });
});
function Query(x) {
    FaresStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            serchs: Ext.getCmp('serchs').getValue(),
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            groupid: document.getElementById("groupid").value
        }
    });
}

Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
        width: document.documentElement.clientWidth,
        height: 800,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'v_id', width: 100, align: 'center'
            },
            { header: "電子郵件", dataIndex: 'vuser_email', width: 400, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);'  onclick='EditUser()'>" + value + "</a>";
                }
            },
            { header: "狀態", dataIndex: 'status', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    //return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.v_id + ")'><img hidValue='0' id='img" + record.data.v_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    return "<img  id='img' src='../../../Content/img/icons/ok.png'/>";
                }
                else {
                    //return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.v_id + ")'><img hidValue='1' id='img" + record.data.v_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    return "<img  id='img' src='../../../Content/img/icons/cross.gif'/>";
                }
            }
            },
            { header: "員工編號", dataIndex: 'emp_id', width: 130, align: 'center' },
            { header: "建立時間", dataIndex: 'screatedate', width: 300, align: 'center' }
           ],
        tbar: [
           { xtype: 'combobox', editable: false, fieldLabel: "查詢條件", labelWidth: 60, id: 'serchs', store: DDLStore, displayField: 'txt', valueField: 'value', value: 0 },
           { xtype: 'textfield', fieldLabel: "查詢內容", id: 'serchcontent', labelWidth:60},
           {
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
           {
               text: RETURN,
               scale: 'small',
               style: { marginLeft: '20px' },
               handler: function () {
                   window.location.href = '/Member/VipUserGroupList';
               }
           
           }
       ], 
        bbar: Ext.create('Ext.PagingToolbar', {
            store: FaresStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        //selModel: sm,
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }

        }

    });


    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    FaresStore.load({ params: { start: 0, limit: 25} });
});

function EditUser() {

    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FaresStore);
    }

}
//更改會員狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Member/UpdateUserState",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                FaresStore.load();
            } 
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                FaresStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            FaresStore.load();
        }
    });
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, FaresStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FaresStore);
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/Promotions/DeletePromotionsAccumulateRate',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            FaresStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

