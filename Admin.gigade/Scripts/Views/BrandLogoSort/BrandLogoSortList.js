var pageSize = 25;
Ext.define('gigade.BrandLogoSort', {
    extend: 'Ext.data.Model',
    fields: [

    { name: "blo_id", type: "int" },
    { name: "brand_id", type: "int" },
    { name: "brand_name", type: "string" },
    { name: "brand_logo", type: "string" },
    { name: "blo_sort", type: "int" },
      { name: "category_id", type: "int" },
    { name: "category_name", type: "string" },
    { name: "user_username", type: "string" },
    { name: "blo_mdate", type: "string" },
    ]
});

BrandLogoSortStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.BrandLogoSort',
    proxy: {
        type: 'ajax',
        url: '/BrandLogoSort/GetBLSList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

BrandLogoSortStore.on('beforeload', function () {
    Ext.apply(BrandLogoSortStore.proxy.extraParams,
    {
        category_id: Ext.getCmp('category_id_ser').getValue(),
        brand_id: Ext.getCmp('brand_id_ser').getValue(),

    });
});



var comType = false;
Ext.define('gigade.CategoryStore2', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'category_id', type: 'int' },
        { name: 'category_name', type: 'string' }
    ]
});
var CategoryStore2 = Ext.create("Ext.data.Store", {
    autoLoad: true,
    model: 'gigade.CategoryStore2',
    proxy: {
        type: 'ajax',
        url: '/BrandLogoSort/GetCategoryStore',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Ext.define('gigade.BrandStore2', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'brand_id', type: 'int' },
        { name: 'brand_name', type: 'string' }
    ]
});
var BrandStore2 = Ext.create("Ext.data.Store", {
    model: 'gigade.BrandStore2',
    proxy: {
        type: 'ajax',
        url: '/BrandLogoSort/GetBrandStore',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

BrandStore2.on('beforeload', function () {
    Ext.apply(BrandStore2.proxy.extraParams,
        {
            category_id: Ext.getCmp('category_id_ser').getValue(),
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("BrandLogoSort").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("BrandLogoSort").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {

    var BrandLogoSort = Ext.create('Ext.grid.Panel', {
        id: 'BrandLogoSort',
        store: BrandLogoSortStore,
        flex: '8.9',
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "編號", dataIndex: 'blo_id', width: 60, align: 'center' },
        { header: "品牌名稱", dataIndex: 'brand_name', width: 150, align: 'center' },
                        {
                            header: "品牌logo",
                            dataIndex: 'brand_logo',
                            width: 80,
                            align: 'center',
                            xtype: 'templatecolumn',
                            tpl: '<a target="_blank" href="{brand_logo}" ><img width=50 name="tplImg" height=50 src="{brand_logo}" /></a>'
                        },
        { header: "排序", dataIndex: 'blo_sort', width: 150, align: 'center' },
        { header: "品牌館分類", dataIndex: 'category_name', width: 150, align: 'center' },
        { header: "異動人員", dataIndex: 'user_username', width: 150, align: 'center' },
          { header: "異動時間", dataIndex: 'blo_mdate', width: 150, align: 'center' },
        ],
        tbar: [
        { xtype: 'button', text: '新增', id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
        { xtype: 'button', text: '編輯', id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: '刪除', id: 'delete', hidden: false, iconCls: 'ui-icon ui-icon-user-delete', disabled: true, handler: onDeleteClick },

         '->',
         {
             xtype: 'combobox',
             fieldLabel: '品牌館分類',
             id: 'category_id_ser',
             displayField: 'category_name',
             valueField: 'category_id',
             labelWidth: 70,
             editable: false,
             emptyText: "請選擇...",
             store: CategoryStore2,
             listeners: {
                 select: function (combo, record) {
                     var m = Ext.getCmp("brand_id_ser");
                     m.clearValue();
                     BrandStore2.removeAll();
                     BrandStore2.load({
                         params: {
                             category_id: Ext.getCmp('category_id_ser').getValue(),
                         }
                     });
                     comType = true;
                 }
             },
         },
          {
              xtype: 'combobox', fieldLabel: '品牌名稱', id: 'brand_id_ser',
              store: BrandStore2,
              labelWidth: 70,
              width: 245,
              margin: '0 0 0 15',
              emptyText: "請選擇...",
              displayField: 'brand_name',
              valueField: 'brand_id',
              queryMode: 'local',
              typeAhead: true,
              listeners: {
                  beforequery: function (qe) {
                      if (comType) {
                          delete qe.combo.lastQuery;
                          BrandStore2.load({
                              params: {
                                  category_id: Ext.getCmp('category_id_ser').getValue(),
                              }
                          });
                          comType = false;
                      }
                  }
              }
          },
         {
             xtype: 'button', text: '查詢', handler: Search
         },
         {
             xtype: 'button', text: '重置', handler: function () {
                 Ext.getCmp('category_id_ser').setValue();
                 Ext.getCmp('brand_id_ser').setValue();
             }
         },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BrandLogoSortStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "共計" + ': {2}',
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
        items: [BrandLogoSort],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                BrandLogoSort.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //EdmContentStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, BrandLogoSortStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("BrandLogoSort").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行");
    } else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行");
    } else if (row.length == 1) {
        editFunction(row[0], BrandLogoSortStore);
    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("BrandLogoSort").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {
    
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.blo_id + "∑";
                }
                Ext.Ajax.request({
                    url: '/BrandLogoSort/DeleteBLS',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        myMask.hide();
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示信息", "刪除成功！");
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                        }
                        BrandLogoSortStore.load();
                    },
                    failure: function () {
                        myMask.hide();
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        BrandLogoSortStore.load();
                    }
                });
            }
        });
    }
}


function Search() {

    BrandLogoSortStore.removeAll();
    var category_id = Ext.getCmp('category_id_ser').getValue();
    var brand_id = Ext.getCmp('brand_id_ser').getValue();
    if (category_id == null && brand_id == null) {
        Ext.Msg.alert("提示信息", "請選擇查詢條件");
        return;
    }
    else if (category_id == null) {
        Ext.Msg.alert("提示信息", "請先選擇品牌館分類");
        //Ext.getCmp('brand_id_ser').setValue();
        return;
    }
    else {
        Ext.getCmp("BrandLogoSort").store.loadPage(1, {
            params: {
                category_id: Ext.getCmp('category_id_ser').getValue(),
                brand_id: Ext.getCmp('brand_id_ser').getValue(),
            }
        });
    }
}


