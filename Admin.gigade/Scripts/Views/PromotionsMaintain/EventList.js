
var CallidForm;
var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.ProPromos', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rid", type: "int" },
        { name: "product_id", type: "string" },
        { name: "event_id", type: "string" },
        { name: "event_type", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "start", type: "string" },
        { name: "end", type: "string" },
        { name: "page_url", type: "string" },
        { name: "kdate", type: "string" },
        { name: "status", type: "int" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
});

var ProPromoStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ProPromos',
    proxy: {
        type: 'ajax',
        //url:controller/fangfaming
        url: '/PromotionsMaintain/GetList',
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
            Ext.getCmp("gdPPromo").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdPPromo").down('#sedit').setDisabled(selections.length == 0);
            Ext.getCmp("gdPPromo").down('#fedit').setDisabled(selections.length == 0);
        }
    }
});

ProPromoStore.on('beforeload', function () {
    Ext.apply(ProPromoStore.proxy.extraParams,
        {
            searchcontent: Ext.getCmp('searchcontent').getValue()
        });
});
Ext.onReady(function () {
    var gdPPromo = Ext.create('Ext.grid.Panel', {
        id: 'gdPPromo',
        store: ProPromoStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: RID, dataIndex: 'rid', width: 60, align: 'center' },
            { header: PRODUCTID, dataIndex: 'product_id', width: 100, align: 'center' },
            { header: EVENTID, dataIndex: 'event_id', width: 100, align: 'center' },
            { header: EVENTTYPE, dataIndex: 'event_type', width: 100, align: 'center' },
            { header: EVENTDESC, dataIndex: 'event_desc', width: 150, align: 'center' },
            { header: START, dataIndex: 'start', width: 160, align: 'center' },
            { header: END, dataIndex: 'end', width: 160, align: 'center' },
            { header: PAGEURL, dataIndex: 'page_url', width: 200, align: 'center' },
            { header: KDATE, dataIndex: 'kdate', width: 160, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: STATUS, dataIndex: 'status', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rid + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.rid + "' src='../../../Content/img/icons/accept.gif'/></a>";

                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rid + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.rid + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           //{ xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: MEDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           { xtype: 'button', text: SMEDIT, id: 'sedit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onSEditClick },
           { xtype: 'button', text: FMEDIT, id: 'fedit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onFEditClick },
           '->',
           {
               xtype: 'textfield', allowBlank: true, id: 'searchcontent', name: 'searchcontent', fieldLabel: SEARCHTYPE, labelWidth: 120
               ,
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
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
          {
              xtype: 'button',
              text: RESET,
              id: 'btn_reset',
              listeners: {
                  click: function () {
                      Ext.getCmp("searchcontent").setValue("");
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProPromoStore,
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
        items: [gdPPromo],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdPPromo.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //   ProPromoStore.load({ params: { start: 0, limit: 25 } });
});

function Query(x) {
    if (Ext.getCmp('searchcontent').getValue() == "") {
        Ext.Msg.alert("提示信息", "請輸入查詢條件！");
        return;
    }
    ProPromoStore.removeAll();
    Ext.getCmp("gdPPromo").store.loadPage(1, {
        params: {
            searchcontent: Ext.getCmp('searchcontent').getValue()
        }
    });
}

/*************************************************************************************批量編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdPPromo").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        //alert(row[0].data.event_id);
        var firstId = row[0].data.event_id;
        for (var i = 0; i < row.length; i++) {
            if (firstId !== row[i].data.event_id) {
                Ext.Msg.alert("提示信息", "所選活動ID不一致!");
                return;
            }
        }
        editFunction(row);
    }
}
/*************************************************************批量啟用*************************************/

onSEditClick = function () {
    var row = Ext.getCmp("gdPPromo").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length >= 1) {
        var rids = "";
        var musers = "";
        var val ="";
        for (var i = 0; i < row.length; i++) {
            rids += row[i].data.rid + ',';
            musers += row[i].data.muser + ',';
            if ($("#img" + row[i].data.rid).attr("hidValue")==0)
            {
                Ext.Msg.alert("提示信息", "選中數據存在已啟用數據，請重新選擇");
                return;
            }   
        }
        $.ajax({
            url: "/PromotionsMaintain/UpStatus",
            data: {
                "id": rids,
                "musers": musers,
                "active": 1
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                if (msg.success == "stop") {
                    Ext.Msg.alert("提示信息", QuanXianInfo);
                }
                else if (msg.success == "true") {
                    ProPromoStore.removeAll();
                    ProPromoStore.load();
                    for (var i = 0; i < row.length; i++) {
                        $("#img" + row[i].data.rid).attr("hidValue", 0);
                        $("#img" + row[i].data.rid).attr("src", "../../../Content/img/icons/accept.gif");
                    }
                }
                else {
                    Ext.Msg.alert("提示信息", "更改失敗");
                }
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
}
/*************************************************************批量禁用*************************************/
onFEditClick = function () {
    var row = Ext.getCmp("gdPPromo").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length >= 1) {
        var rids = "";
        var musers = "";
        for (var i = 0; i < row.length; i++) {
            rids += row[i].data.rid + ',';
            musers += row[i].data.muser + ',';
            if ($("#img" + row[i].data.rid).attr("hidValue") == 1) {
                Ext.Msg.alert("提示信息", "選中數據存在已禁用數據，請重新選擇");
                return;
            }
        }
        Ext.Ajax.request({
            url: "/PromotionsMaintain/UpStatus",
            method: 'post',
            params: {
                id: rids,
                "musers": musers,
                "active": 0
            },
            success: function (msg) {
                for (var i = 0; i < row.length; i++) {
                    $("#img" + row[i].data.rid).attr("hidValue", 1);
                    $("#img" + row[i].data.rid).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
                ProPromoStore.load();
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);

            }
        });
    }
}



//更改狀態(啟用或者禁用)
function UpdateActive(id, muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/PromotionsMaintain/UpStatus",
        data: {
            "id": id,
            "muser": muser,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert("提示信息", QuanXianInfo);
            }
            else if (msg.success == "true") {
                ProPromoStore.removeAll();
                ProPromoStore.load();
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
            }
            else {
                Ext.Msg.alert("提示信息", "更改失敗");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            ProPromoStore.load();
        }
    });
}





