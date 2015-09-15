var CallidForm;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.MarketProductMap', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "map_id", type: "int" },//流水編號
        { name: "product_category_id", type: "int" },//吉甲地類別編號
        { name: "market_category_id", type: "int" },//美安類別編號
   
        { name: "modified", type: "string" },//修改時間
      
        { name: "muser", type: "string" },//修改人

        { name: "market_category_code", type: "string" },//美安類別數字編號
        { name: "market_category_name", type: "string" },//美安類別名稱

        { name: "category_name", type: "string" },//吉甲地類別
    ]
});

//促銷贈品表Store
var MarketProductMapStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.MarketProductMap',
    proxy: {
        type: 'ajax',
        url: '/MarketCategory/GetMarketProductMapList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {

            Ext.getCmp("MarketProductMap").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("MarketProductMap").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
MarketProductMapStore.on('beforeload', function () {
    Ext.apply(MarketProductMapStore.proxy.extraParams, {
        search: Ext.getCmp('search').getValue()
     //   SearchTime: Ext.getCmp('ddlSel').getValue()
    })

});



function Query(x) {
    MarketProductMapStore.removeAll();
    Ext.getCmp("MarketProductMap").store.loadPage(1);
}

Ext.onReady(function () {
   
    var MarketProductMap = Ext.create('Ext.grid.Panel', {
        id: 'MarketProductMap',
        store: MarketProductMapStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
                { header: MAP_ID, dataIndex: 'map_id', width: 150, align: 'center' },
                { header: GIGADE_CATEGORY_ID, dataIndex: 'product_category_id', width: 150, align: 'center' },
                { header: GIGADE_CATEGORY_NAME, dataIndex: 'category_name', width: 150, align: 'center' },
                { header: MARKET_CATEGORY_CODE, dataIndex: 'market_category_code', width: 150, align: 'center' },
                { header: MARKET_CATEGORY_NAME, dataIndex: 'market_category_name', width: 150, align: 'center' },
              
                { header: MODIFIED, dataIndex: 'modified', width: 150, align: 'center' },
            
                { header: MUSER, dataIndex: 'muser', width: 150, align: 'center' }
             
           
        ],
        tbar: [
          { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
          { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          { xtype: 'button', text: REMOVE, id: 'delete', iconCls: 'icon-remove', disabled: true, handler: RemoveClick },
           '->',
            //{
            //    xtype: 'combobox', editable: false, fieldLabel: "查詢條件", labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: '1'
            //}
             { xtype: 'textfield', fieldLabel: NUM_NAME_NO, labelWidth: 100, id: 'search' }
            , {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: MarketProductMapStore,
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
        }
          , selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [MarketProductMap],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                MarketProductMap.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    MarketProductMapStore.load({ params: { start: 0, limit: 25 } });
});
//*********新增********//
onAddClick = function () {
    editFunction(null, MarketProductMapStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("MarketProductMap").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else {
        editFunction(row[0], MarketProductMapStore);
    }
}
//**************刪除****************//

RemoveClick = function () {
    var row = Ext.getCmp("MarketProductMap").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.map_id + ',';
                }

                Ext.Ajax.request({
                    url: '/MarketCategory/DeleteMarketProductMap',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        MarketProductMapStore.load();

                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                        MarketProductMapStore.load();
                    }
                });
            }
        });
    }
}

function TranToSetGift(eventId) {
    var url = '/NewPromo/NewPromoPresent?Event_Id=' + eventId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#Sms');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'Sms',
        title: GIFTSET,
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}




//更改活動狀態(啟用前先檢查該活動是否具有有效贈品，是則啟用，否則提示請設定有效贈品)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    var giftNum = 0;
    Ext.Ajax.request({
        url: '/NewPromo/GetNewPromoPresent',
        method: 'post',
        async: false,
        params: { event_id: id },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                giftNum = result.count;
            }

        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
    if (giftNum > 0 || activeValue == "0") {
        $.ajax({
            url: "/NewPromo/UpdateActiveQuestion",
            data: {
                "event_id": id,
                "active": activeValue
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                MarketProductMap.load();
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, GIFTTIP);
        return;
    }
}

function ExportExeclUserMessage(id) {
    window.open("/NewPromo/ExportNewPromoRecordList?event_id=" + id);
}