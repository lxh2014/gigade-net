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
Ext.define('gigade.Gwj', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" },
        { name: "master_id", type: "string" },
        { name: "now_time", type: "int" },
        { name: "user_name", type: "string" },
        { name: "type_description", type: "string" },
        { name: "master_total", type: "int" },
        { name: "master_balance", type: "int" },
        { name: "smaster_start", type: "string" },
        { name: "smaster_end", type: "string" },
        { name: "smaster_createtime", type: "string" },
        { name: "master_start", type: "int" },
        { name: "master_end", type: "int" },
        { name: "master_createtime", type: "int" },
        { name: "bonus_type", type: "string" },
        { name: "master_note", type: "string" }
    ]
});

var BonusStore = Ext.create('Ext.data.Store', {
    //  autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Gwj',
    proxy: {
        type: 'ajax',
        url: '/Member/BonusSearchList',
        reader: {
            type: 'json',
            root: 'data', //在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
    //,
    //autoLoad: true
});
//頁面加載時判斷是否有數據
BonusStore.on('load', function (store, records, options) {
    var totalcount = records.length;
    if (totalcount == 0) {
        Ext.MessageBox.alert(INFORMATION, "～搜尋不到資料～");
    }
});

//使用者Model
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "name", type: "string" },
        { name: "callid", type: "string"}]
});

var ManageUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Fgroup/QueryCallid',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdGwj").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

BonusStore.on('beforeload', function () {
    Ext.apply(BonusStore.proxy.extraParams, {
        uid: document.getElementById("userid").value
    });
});


Ext.onReady(function () {
    var gdGwj = Ext.create('Ext.grid.Panel', {
        id: 'gdGwj',
        store: BonusStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "購物金編號", dataIndex: 'master_id', width: 100, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 100, align: 'center' },
            { header: "用戶名", dataIndex: 'user_name', width: 100, align: 'center', hidden: true },
            { header: "摘要", dataIndex: 'type_description', width: 100, align: 'center' },
            { header: "總額", dataIndex: 'master_total', width: 100, align: 'center' },
            { header: "結餘", dataIndex: 'master_balance', width: 100, align: 'center' },
            //{ header: "NowTime", dataIndex: 'now_time', width: 120, align: 'center', hidden: true }, //, hidden: true 
            {header: "開始日", dataIndex: 'smaster_start', width: 120, align: 'center' },
            { header: "結束日", dataIndex: 'smaster_end', width: 120, align: 'center' },
            { header: "發放日", dataIndex: 'smaster_createtime', width: 120, align: 'center' },
            { header: "狀態 ", dataIndex: 'now_time', width: 100, align: 'center', renderer: showbonus_status },
            { header: "類型", dataIndex: 'bonus_type', width: 100, align: 'center', renderer: showbonus_type },
            { header: "備註 ", dataIndex: 'master_note', width: 100, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', handler: onEditClick },
            '->',
            {
                xtype: "button",
                text: "返回會員查詢列表",
                id: "goback",
                handler: function () {
                    //window.location.href = "/Member/UsersListIndex";
                    history.back();
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BonusStore,
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
        items: [gdGwj],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdGwj.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ToolAuthority();
    BonusStore.load({ params: { start: 0, limit: 25} });
});

function showbonus_type(val) {
    switch (val) {
        case "1":
            return "購物金";
            break;
        case "2":
            return "抵用券";
            break;
        default:
            return "數據異常";
            break;
    }
}

function showbonus_status(value, cellmeta, record, rowIndex, columnIndex, store) {

    if (value < record.data.master_start ) {
        return "尚未開通";
    } else if (value > record.data.master_end) {
        return "已過期";
    } else if (record.data.master_total <= record.data.master_balance) {
        return "未使用";
    } else if (record.data.master_balance > 0) {
        return "尚餘點數";
    } else {
        return "已用完";
    }
    //    switch (val) {
    //        case "1":
    //            return "尚未開通";
    //            break;
    //        case "2":
    //            return "已過期";
    //            break;
    //        case "3":
    //            return "未使用";
    //            break;
    //        case "4":
    //            return "尚餘點數";
    //            break;
    //        case "5":
    //            return "已用完";
    //            break;
    //        case "6":
    //            return "已取消";
    //            break;
    //        default:
    //            return "數據異常";
    //            break;
    //    }
}



/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //    addWin.show();
    editFunction(null, BonusStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdGwj").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], BonusStore);
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdGwj").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
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
                            BonusStore.load();
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