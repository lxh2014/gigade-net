//庫調參數設定列表頁

var pageSize = 25;
//Model
Ext.define('gigade.TPparametersrc', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Rowid", type: "int" },            //編號
        { name: "ParameterType", type: "string" }, //參數代碼
        { name: "parameterName", type: "string" }, //參數名稱
        { name: "ParameterCode", type: "string" }, //參數名稱
        { name: "remark", type: "string" },        //備註
        { name: "Kdate", type: "string" },         //備註
        { name: "TopValue", type: "string" }       //上級
    ]
});
    //數據
    var TPStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        pageSize: pageSize,
        model: 'gigade.TPparametersrc',
        proxy: {
            type: 'ajax',
            url: '/WareHouse/GetTPList',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
    });

    TPStore.on('beforeload', function () {
        Ext.apply(TPStore.proxy.extraParams, {
            searchcontent: Ext.getCmp('searchcontent').getValue()
        });
    });

    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("TPgrid").down('#edit').setDisabled(selections.length == 0);
                Ext.getCmp("TPgrid").down('#remove').setDisabled(selections.length == 0);
            }
        }
    });

    function Query(x) {
        TPStore.removeAll();
        Ext.getCmp("TPgrid").store.loadPage(1, {
            params: {
                searchcontent: Ext.getCmp('searchcontent').getValue()
            }
        });
    }
    Ext.onReady(function () {
        var TPgrid = Ext.create('Ext.grid.Panel', {
            id: 'TPgrid',
            store: TPStore,
            width: document.documentElement.clientWidth,
            columnLines: true,
            frame: true,
            columns: [
                { header: "編號", dataIndex: 'Rowid', width: 150, align: 'center' },
                { header: "父級名", dataIndex: 'TopValue', width: 100, align: 'center' },
                { header: "類別代碼", dataIndex: 'ParameterType', width: 150, align: 'center' },
                { header: "類別名稱", dataIndex: 'parameterName', width: 150, align: 'center' },
                { header: "Code代碼", dataIndex: 'ParameterCode', width: 150, align: 'center' },
                { header: "備註", dataIndex: 'remark', width: 100, align: 'center' },
                { header: "創建時間", dataIndex: 'Kdate', width: 100, align: 'center' }
            ],
            tbar: [
                { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },//disabled: true,
                { xtype: 'button', text: '編輯', id: 'edit', hidden: false, disabled: true, iconCls: 'icon-user-edit', handler: onEditClick },
                { xtype: 'button', text: "刪除", id: 'remove', hidden: false, disabled: true, iconCls: 'icon-user-remove', handler: onRemoveClick },
                       '->',
           { xtype: 'textfield', allowBlank: true, fieldLabel: "查詢", id: 'searchcontent', name: 'searchcontent', labelWidth: 60 },
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
            store: TPStore,
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
        items: [TPgrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                TPgrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    TPStore.load({ params: { start: 0, limit: 25 } });
});

/************************新增/************************/
onAddClick = function () {
    addFunction(null, TPStore);
}
/************************編輯/************************/
onEditClick = function () {
    var row = Ext.getCmp("TPgrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        UpdFunction(row[0], TPStore);
    }
}
/************************刪除/************************/
onRemoveClick = function () {
    var row = Ext.getCmp("TPgrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.Rowid + '|';
                }
                Ext.Ajax.request({
                    url: '/WareHouse/DeleteTp',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            TPStore.load(1);
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
