var pageSize = 25;
var fatherid = "0";
var fathername = "ROOT";

Ext.define('gigade.MarketCategoryModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "market_category_id", type: "int" },
        { name: "market_category_father_id", type: "int" },
        { name: "market_category_father_name", type: "string" },
        { name: "market_category_code", type: "string" },
        { name: "market_category_name", type: "string" },
        { name: "market_category_sort", type: "int" },
        { name: "market_category_status", type: "int" },
       // { name: "muser", type: "int" },
        { name: "muser_name", typr: "string" },
        { name: "modified", type: "string" }

    ]
});

var MarketCategoryStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.MarketCategoryModel',
    proxy: {
        type: 'ajax',
        url: '/MarketCategory/GetMarketCategoryList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdMarket").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdMarket").down('#remove').setDisabled(selections.length == 0);
        }
    }
});


//加載前先獲取關鍵字的值
MarketCategoryStore.on('beforeload', function () {
    Ext.apply(MarketCategoryStore.proxy.extraParams, {
        father_id: fatherid
    })

});
function Search() {
    MarketCategoryStore.removeAll();
    Ext.getCmp("gdMarket").store.loadPage(1, {
        params: {
            father_id: fatherid,
            search: Ext.getCmp("search").getValue()
        }
    });
}
//跳轉子類別
function Query(father_id, father_neme) {
    fatherid = father_id;
    if (father_id == "0") {
        fathername = "ROOT";
    }
    else {
        fathername = father_neme;
    }
    MarketCategoryStore.removeAll();
    Ext.getCmp("gdMarket").store.loadPage(1, {
        params: {
            father_id: father_id
        }
    });
}

function SetMap(cid, cname) {//關係設定
    setMarketProductMap(cid, cname);
}
//頁面載入
Ext.onReady(function () {
    var gdMarket = Ext.create('Ext.grid.Panel', {
        id: 'gdMarket',
        store: MarketCategoryStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: MARKET_CATEGORY_ID, dataIndex: 'market_category_id', width: 100, align: 'center' },
            { header: MARKET_CATEGORY_NAMES, dataIndex: 'market_category_name', width: 200, align: 'center' },
            { header: CATEGORY_CODE, dataIndex: 'market_category_code', width: 100, align: 'center' },
            {
                header: MARKET_CATEGORY_FATHER_NAMES, dataIndex: 'market_category_father_name', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.market_category_father_id == 0) {
                        return "ROOT";
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: CATEGORY_SORT, dataIndex: 'market_category_sort', width: 100, align: 'center' },
            { header: MUSER, dataIndex: 'muser_name', width: 120, align: 'center' },
            { header: MODIFIED, dataIndex: 'modified', width: 150, align: 'center' },

              {
                  header: FUNCTION,
                  dataIndex: 'market_category_id',
                  hidden: false,
                  width: 200,
                  align: 'center',
                  id: 'childCategory',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      return "<a href='javascript:void(0);' onclick='Query(\"" + value + "\",\"" + record.data.market_category_name + "\")'>" + CHILDTYPE + "</a>&nbsp;&nbsp;<a href='javascript:void(0);' onclick='SetMap(\"" + value + "\",\"" + record.data.market_category_name + "\")'> " + CATEGORY_RELATIONSHIP + "</a>";
                  }
              },
               {
                   header: MARKET_CATEGORY_STATUS,
                   dataIndex: 'market_category_status',
                   id: 'controlactive',
                   hidden: false,
                   align: 'center',
                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                       if (value == 1) {
                           return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.market_category_id + "," + record.data.market_category_father_id + ")'><img hidValue='0' id='img" + record.data.market_category_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                       } else {
                           return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.market_category_id + "," + record.data.market_category_father_id + ")'><img hidValue='1' id='img" + record.data.market_category_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                       }
                   }
               },
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
           { xtype: 'button', text: UPPER_STORY, id: 'back', hidden: false, iconCls: 'icon-search', handler: onBack },

            '->',
            { xtype: 'textfield', fieldLabel: NUM_NAME_NO, labelWidth: 100, id: 'search' },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Search
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: MarketCategoryStore,
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
        items: [gdMarket],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdMarket.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    MarketCategoryStore.load({ params: { start: 0, limit: 25 } });
});


//添加
onAddClick = function () {
    editFunction(null, MarketCategoryStore, fatherid, fathername);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdMarket").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var fname = "";
        if (row[0].data.market_category_father_id == 0) {
            fname = "ROOT";
        }
        else {
            fname = row[0].data.market_category_father_name;
        }
        editFunction(row[0], MarketCategoryStore, row[0].data.market_category_father_id, fname);
    }
}
onRemoveClick = function () {

    var row = Ext.getCmp("gdMarket").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = "";
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.market_category_id + '|';

                }

                Ext.Ajax.request({
                    url: '/MarketCategory/DeleteMarketCategory',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == 0) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, DELETETIP);
                            }
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
                MarketCategoryStore.remove();
                MarketCategoryStore.load({
                    params: {
                        father_id: fatherid
                    }
                });
            }
        });
    }
}
//返回上一頁
onBack = function () {
    if (fatherid != "0") {
        $.ajax({
            url: "/MarketCategory/GetLastFatherId",
            data: {
                "fid": fatherid
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                MarketCategoryStore.remove();
                fatherid = msg.result;
                if (fatherid == "0") {
                    fathername = "ROOT";
                }
                else {
                    fathername = msg.fatherName;
                }
                MarketCategoryStore.loadPage(1, { params: { father_id: msg.result } });
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }

}


//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, fatherId) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/MarketCategory/UpdateActiveMarketCategory",
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

            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");

            }
            MarketCategoryStore.remove();
            MarketCategoryStore.load({ params: { father_id: fatherId } });
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
