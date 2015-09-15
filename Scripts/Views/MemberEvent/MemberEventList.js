var CallidForm;
var pageSize = 25;
/**************************************************************************************/
//自定义VTypes类型，验证日期范围  
Ext.apply(Ext.form.VTypes, {
    dateRange: function (val, field) {
        if (field.dateRange) {
            var beginId = field.dateRange.begin;
            this.beginField = Ext.getCmp(beginId);
            var endId = field.dateRange.end;
            this.endField = Ext.getCmp(endId);
            var beginDate = this.beginField.getValue();
            var endDate = this.endField.getValue();
        }
        if (beginDate != null && endDate != null) {
            if (beginDate <= endDate) {
                return true;
            } else {
                return false;
            }
        }
        else {
            return true;
        }

    },
    //验证失败信息  
    dateRangeText: '日期範圍不正確'
});
//密碼Model
Ext.define('gigade.MemberEvent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowID", type: "string" },
        { name: "me_name", type: "string" },
        { name: "me_desc", type: "string" },
        { name: "me_startdate", type: "string" },
        { name: "me_enddate", type: "string" },
        { name: "et_id", type: "string" },
        { name: "me_birthday", type: "string" },
        { name: "event_id", type: "string" },
        { name: "me_big_banner", type: "string" },
        { name: "me_bonus_onetime", type: "int" },
        { name: "ml_code", type: "string" },
        { name: "me_status", type: "string" },
        { name: "k_date", type: "string" },
        { name: "memberName", type: "string" },
        { name: "et_name", type: "string" },
        { name: "ml_name", type: "string" },
        { name: "et_date_parameter", type: "string" },
        { name: "et_starttime", type: "string" },
        { name: "et_endtime", type: "string" },
          { name: "me_banner_link", type: "string" },
             { name: "s_me_banner_link", type: "string" },
        
    ]
});

var MemberEventStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.MemberEvent',
    proxy: {
        type: 'ajax',
        url: '/MemberEvent/GetMemberEventList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//出货日期
var dayStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '1', "value": "1" },
        { "txt": '2', "value": "2" },
        { "txt": '3', "value": "3" },
        { "txt": '4', "value": "4" },
        { "txt": '5', "value": "5" },
        { "txt": '6', "value": "6" },
        { "txt": '7', "value": "7" },
        { "txt": '8', "value": "8" },
        { "txt": '9', "value": "9" },
        { "txt": '10', "value": "10" },
        { "txt": '11', "value": "11" },
        { "txt": '12', "value": "12" },
        { "txt": '13', "value": "13" },
        { "txt": '14', "value": "14" },
        { "txt": '15', "value": "15" },
        { "txt": '16', "value": "16" },
        { "txt": '17', "value": "17" },
        { "txt": '18', "value": "18" },
        { "txt": '19', "value": "19" },
        { "txt": '20', "value": "20" },
        { "txt": '21', "value": "21" },
        { "txt": '22', "value": "22" },
        { "txt": '23', "value": "23" },
        { "txt": '24', "value": "24" },
        { "txt": '25', "value": "25" },
        { "txt": '26', "value": "26" },
        { "txt": '27', "value": "27" },
        { "txt": '28', "value": "28" },
        { "txt": '29', "value": "29" },
        { "txt": '30', "value": "30" },
        { "txt": '31', "value": "31" }
    ]
});
//下拉框
Ext.define("gigade.ManageUser", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "string" },
        { name: "user_name", type: "string" }]
});
var muStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ManageUser',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/SecretInfo/GetManagerUser",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdMemberEvent").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
MemberEventStore.on('beforeload', function () {
    Ext.apply(MemberEventStore.proxy.extraParams, {
        search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue()
    });
});
Ext.onReady(function () {
    Ext.tip.QuickTipManager.init();
    var gdMemberEvent = Ext.create('Ext.grid.Panel', {
        id: 'gdMemberEvent',
        store: MemberEventStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "活動編號", dataIndex: 'rowID', width: 70, align: 'center' },
            {
                header: "活動名稱", dataIndex: 'me_name', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                    return value;
                }
            },
            {
                header: "活動描述", dataIndex: 'me_desc', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                    return value;
                }
            },
            { header: "活動開始時間", dataIndex: 'me_startdate', width: 100, align: 'center' },
            { header: "活動結束時間", dataIndex: 'me_enddate', width: 100, align: 'center' },
            {
                header: "活動類型", dataIndex: 'et_name', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case "DD":
                            return "每日"
                            break;
                        case "WW":
                            return "每週"
                            break;
                        case "MM":
                            return "每月"
                            break;
                    }
                }
            },
            {
                header: "活動圖", dataIndex: 'me_banner_link', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a target="_blank", href="' + record.data.s_me_banner_link + '"><img width="50px" height="50px" src="' + record.data.s_me_banner_link + '" /></a>'
                }
            },
            {
                header: "連接地址", dataIndex: 'me_banner_link', width: 200, align: 'center'
                
            },
            {
                header: "時間參數", dataIndex: 'et_date_parameter', width: 80, align: 'center'
                //,
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                //{
                //    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                //    var pa = record.data.et_date_parameter.toString().split(',');
                //    var result = "";
                //    for (var i = 0; i < pa.length; i++)
                //    {
                //        if (record.data.et_name == "DD")
                //        {
                //            result += "、";
                //        }
                //        if (record.data.et_name == "WW")
                //        {
                //            result += "每週" + pa[i] + "、";
                //        }
                //        if (record.data.et_name == "MM")
                //        {
                //            result += "每月" + pa[i] + "日" + "、";
                //        }

                //    }
                //    return result.substr(0, result.length - 1);
                //}
            },
            { header: "活動每天開始時間", dataIndex: 'et_starttime', width: 110, align: 'center' },
            { header: "活動每天結束時間", dataIndex: 'et_endtime', width: 110, align: 'center' },
            {
                header: "壽星會員獨享", dataIndex: 'me_birthday', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "是";
                    } else {
                        return "否";
                    }
                }
            },
            { header: "促銷編號", dataIndex: 'event_id', width: 80, align: 'center' },
            {
                header: "是否限領一次", dataIndex: 'me_bonus_onetime', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "是";
                    } else {
                        return "否";
                    }
                }
            },
            {
                header: "會員對象群組", dataIndex: 'ml_code', width: 80, align: 'center'
                //,
                //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                //{
                //    cellmeta.style = 'overflow:visible;padding:3px 3px 3px 5px;white-space:normal';
                //    if (value == "0")
                //    {
                //        return "不分";
                //    }
                //    else
                //    {
                //        var ml_code = record.data.ml_code.toString().split(',');
                //        var result = "";
                //        for (var i = 0; i < ml_code.length; i++)
                //        {
                //            result += kv[ml_code[i]] + '、';
                //        }
                //        return result.substr(0, result.length - 1);
                //    }
                //}
            },
            { header: "創建時間", dataIndex: 'k_date', width: 140, align: 'center' },
            {
                header: "活動狀態", dataIndex: 'me_status', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowID + ")'><img hidValue='0' id='img" + record.data.rowID + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowID + ")'><img hidValue='1' id='img" + record.data.rowID + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }

        ],
        tbar: [
         { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          '->',
                {
                    xtype: 'datetimefield',
                    allowBlank: true,
                    id: 'timestart',
                    margin: "0 5 0 0",
                    editable: false,
                    name: 'serchcontent',
                    fieldLabel: '創建時間',
                    labelWidth: 60,
                    value: new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()),
                    dateRange: { begin: 'timestart', end: 'timeend' },
                    vtype: 'dateRange'
                },

             {
                 xtype: 'datetimefield',
                 allowBlank: true,
                 editable: false,
                 id: 'timeend',
                 margin: "0 5 0 0",
                 name: 'serchcontent',
                 fieldLabel: '到',
                 labelWidth: 15,
                 value: new Date(),
                 dateRange: { begin: 'timestart', end: 'timeend' },
                 vtype: 'dateRange'
             },
         {
             xtype: 'textfield', fieldLabel: "編號/名稱/描述", labelWidth: 120, id: 'search_content', listeners: {
                 specialkey: function (field, e) {
                     // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                     // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                     if (e.getKey() == e.ENTER) {
                         Query(1);
                     }
                 }
             }
         },
         {
             text: SEARCH,
             iconCls: 'icon-search',
             id: 'btnQuery',
             handler: Query
         },
         {
             xtype: 'button',
             text: '重置',
             id: 'btn_reset',
             iconCls: 'ui-icon ui-icon-reset',
             listeners: {
                 click: function () {
                     Ext.getCmp('search_content').setValue("");
                     Ext.getCmp('timestart').setValue(new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()));
                     Ext.getCmp('timeend').setValue(new Date());
                     Query();
                 }
             }
         }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: MemberEventStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
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
        layout: 'fit',
        items: [gdMemberEvent],
        renderTo: Ext.getBody(),
      //  autoScroll: true,
        listeners: {
            resize: function () {
                gdMemberEvent.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //MemberEventStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query(x) {
    MemberEventStore.removeAll();
    Ext.getCmp("gdMemberEvent").store.loadPage(1, {
        params: {
            search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue(),
            timestart: Ext.getCmp('timestart').getValue(),
            timeend: Ext.getCmp('timeend').getValue()

        }
    });
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editMemberEventFunction(null, MemberEventStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdMemberEvent").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editMemberEventFunction(row[0], MemberEventStore);
    }
}
//更改狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/MemberEvent/UpdateState",
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
                MemberEventStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                MemberEventStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            MemberEventStore.load();
        }
    });
}







