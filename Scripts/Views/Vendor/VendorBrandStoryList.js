pageSize = 25;

Ext.define('gigade.VendorBrand', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "brand_id", type: "int" },
    { name: "brand_name", type: "string" },
    { name: "brand_story_text", type: "string" },
    { name: "story_created", type: "string" },
    { name: "story_createname", type: "string" },
    { name: "story_createdate", type: "string" }
    ]
});
var VendorBrandStory = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.VendorBrand',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorBrandStory',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "Brand_Id", type: "string" },
    { name: "Brand_Name", type: "string" }]
});

//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    //filterOnLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Product/GetVendorBrand",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data' 
        }
    }
});
//供應商Model
Ext.define("gigade.Vendor", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "vendor_id", type: "string" },
    { name: "vendor_name_simple", type: "string" },
    { name: "vendor_status", type: "int" },
    { name: "brand_status", type: "int" }
    ]
});
//供應商Store
var VendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Product/GetVendor",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var CBoxStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "全部", "value": "0" },
    { "txt": "未編輯", "value": "1" },
    { "txt": "已編輯", "value": "2" }
    ]
});
//供應商狀態
var VendorBoxStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "全部", "value": "0" },
    { "txt": "已啟用", "value": "1" },
    { "txt": "未啟用", "value": "2" },
    ]
});
//品牌狀態
var ProductBoxStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "全部", "value": "0" },
    { "txt": "顯示", "value": "1" },
    { "txt": "隱藏", "value": "2" }
    ]
});

//VendorBrandStory.on('load', function (store, records, options) {
//    var totalcount = records.length;
//    if (totalcount == 0) {
//        Ext.MessageBox.alert(INFORMATION, "～目前无資料～");
//    }
//});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdList").down('#open').setDisabled(selections.length == 0);
        }
    }
});
VendorBrandStory.on('beforeload', function () {
    Ext.apply(VendorBrandStory.proxy.extraParams,
    {
        searchContent: Ext.getCmp('searchContent') == null ? "" : Ext.getCmp('searchContent').getValue(),
        searchVendor: Ext.getCmp('searchVendor').getValue(),
        brandid: Ext.getCmp('brandid').getValue(),
        start_date: Ext.getCmp('start').getValue(),
        end_date: Ext.getCmp('end').getValue(),
        createname: Ext.getCmp('createname').getValue(),
        searchState: Ext.getCmp('searchState').getValue(),
        vendorState: Ext.getCmp('vendorState').getValue(),
        brandState: Ext.getCmp('brandState').getValue()
    });
});
Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 150,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                fieldLabel: '供應商',
                labelWidth: 60,
                defaultListConfig: {              //取消loading的Mask
                    loadMask: false,
                    loadingHeight: 70,
                    minWidth: 70,
                    maxHeight: 300,
                    shadow: "sides"
                },
                editable: false,
                // width: 190,
                margin: '0 10 3 0',
                id: 'searchVendor',
                name: 'searchVendor',
                store: VendorStore,
                displayField: 'vendor_name_simple',
                valueField: 'vendor_id',
                typeAhead: true,
                forceSelection: true,
                emptyText: SELECT,
                listeners: {
                    select: function (records) {
                        Ext.getCmp('brandid').reset();
                        brandStore.load({ params: { vendor_id: Ext.getCmp('searchVendor').getValue() } });
                    }
                }
            },
            {//品牌
                xtype: 'combobox',
                fieldLabel: '品牌',
                labelWidth: 35,
                defaultListConfig: {              //取消loading的Mask
                    loadMask: false,
                    loadingHeight: 70,
                    minWidth: 70,
                    maxHeight: 300,
                    shadow: "sides"
                },
                id: 'brandid',
                name: 'brandid',
                colName: 'brandid',
                editable: true,
                store: brandStore,
                margin: '0 5 3 12',
                queryMode: 'local',
                displayField: 'Brand_Name',
                valueField: 'Brand_Id',
                typeAhead: true,
                triggerAction: 'all',
                forceSelection: true,
                emptyText: '請選擇'
            },
            {
                xtype: 'combobox',
                id: 'searchState',
                fieldLabel: '編輯狀態',
                labelWidth: 60,
                margin: '0 10 0 0',
                queryMode: 'local',
                editable: false,
                store: CBoxStore,
                displayField: 'txt',
                valueField: 'value',
                value: 0
            }
            ]
        }, {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                 {
                     xtype: 'combobox',
                     id: 'vendorState',
                     fieldLabel: '供應商狀態',
                     labelWidth: 100,
                     width: 215,
                     margin: '0 5 3 0',
                     queryMode: 'local',
                     editable: false,
                     store: VendorBoxStore,
                     displayField: 'txt',
                     valueField: 'value',
                     value: 0
                 },
            {
                xtype: 'combobox',
                id: 'brandState',
                fieldLabel: '品牌狀態',
                labelWidth: 60,
                margin: '0 5 3 5',
                queryMode: 'local',
                editable: false,
                store: ProductBoxStore,
                displayField: 'txt',
                valueField: 'value',
                value: 0
            }, {
                xtype: 'textfield', fieldLabel: "品牌編號/名稱", labelWidth: 90, margin: '0 5 3 0', id: 'searchContent', width: 215, listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Search(1);
                        }
                    }
                }
            }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                 {
                     xtype: 'textfield',
                     id: 'createname',
                     name: 'createname',
                     margin: '0 10 3 0',
                     labelWidth: 60,
                     fieldLabel: '建立人',
                     editable: false,
                     listeners: {
                         specialkey: function (field, e) {
                             if (e.getKey() == e.ENTER) {
                                 Search(1);
                             }
                         }
                     }
                 },

                    {
                        xtype: 'datetimefield',
                        fieldLabel: "建立時間",
                        id: 'start',
                        name: 'start',
                        margin: '0 5px 0 0',
                        labelWidth: 60,
                        format: 'Y-m-d H:i:s',
                        time: { hour: 00, min: 00, sec: 00 },//標記結束時間00:00:00
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start");
                                var end = Ext.getCmp("end");
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                } else if (start.getValue() > end.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                    end.setValue(setNextMonth(start.getValue(), 1)); 
                                }
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }

                    },
                    {
                        xtype: 'displayfield',
                        value: '~',
                        margin: '0 11px'
                    },
                    {
                        xtype: 'datetimefield',
                        id: 'end',
                        name: 'end',
                        margin: '0 5px',
                        format: 'Y-m-d H:i:s',
                        editable: false,
                        time: { hour: 23, min: 59, sec: 59 },//標記結束時間23:59:59                       
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start");
                                var end = Ext.getCmp("end");
                                var s_date = new Date(start.getValue());
                                var now_date = new Date(end.getValue());
                                if (start.getValue() != "" && start.getValue() != null) {
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                        //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }

                                } else {
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    }
            ]
        },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                            {
                                xtype: 'button',
                                margin: '0 10 0 130',
                                iconCls: 'ui-icon ui-icon-search-2',
                                text: "查詢",
                                handler: Search
                            },
                      {
                          xtype: 'button',
                          text: '重置',
                          id: 'btn_reset',
                          iconCls: 'ui-icon ui-icon-reset',
                          listeners: {
                              click: function () {
                                  Ext.getCmp('searchVendor').setValue('');
                                  Ext.getCmp('brandid').setValue('');
                                  Ext.getCmp('start').setValue(null);
                                  Ext.getCmp('end').setValue(null);
                                  brandStore.removeAll();
                                  brandStore.load();
                                  Ext.getCmp('searchContent').setValue('');
                                  Ext.getCmp('searchState').setValue('0');
                                  Ext.getCmp('createname').setValue('');
                                  Ext.getCmp('vendorState').setValue('0');
                                  Ext.getCmp('brandState').setValue('0');

                              }
                          }
                      }
                  ]
              }

        ]
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: VendorBrandStory,
        //width: document.documentElement.clientWidth,
        columnLines: true,
        // height: document.documentElement.clientHeight - 150,
        frame: true,
        columns: [
        { header: "品牌編號", dataIndex: 'brand_id', flex: 1, align: 'center' },
        { header: "品牌名稱", dataIndex: 'brand_name', flex: 4, align: 'center' },
        { header: "品牌故事", dataIndex: 'brand_story_text', hidden: true },
        { header: "建立人", dataIndex: 'story_createname', flex: 1, align: 'center' },
        { header: "建立時間", dataIndex: 'story_createdate', flex: 1, align: 'center' }
        ],
        tbar: [
        { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "開啟前台品牌頁面", id: 'open', disabled: true, handler: VendorBrandPreview },
        { xtype: 'button', id: 'exportfile', text: '匯出', iconCls: 'icon-excel', handler: ExportFile }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorBrandStory,
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
        layout: 'vbox',
        items: [frm, gdList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                gdList.height = document.documentElement.clientHeight - 150;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //  VendorBrandStory.load({ params: { start: 0, limit: 25 } });
})

function Search() {
    VendorBrandStory.removeAll();
    Ext.getCmp("gdList").store.loadPage(1);
}
ExportFile = function () {
    Ext.MessageBox.show({
        msg: 'Loading....',
        wait: true
    });

    Ext.Ajax.request({
        url: '/Vendor/ExportFile',
        timeout: 900000,
        params: {
            searchVendor: Ext.getCmp('searchVendor').getValue(),
            brandid: Ext.getCmp('brandid').getValue(),
            searchContent: Ext.getCmp('searchContent').getValue(),
            createname: Ext.getCmp('createname').getValue(),
            start_date: Ext.getCmp('start').getValue(),
            end_date: Ext.getCmp('end').getValue(),
            searchState: Ext.getCmp('searchState').getValue(),
            vendorState: Ext.getCmp('vendorState').getValue(),
            brandState: Ext.getCmp('brandState').getValue()
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                window.location = '../../ImportUserIOExcel/' + result.ExcelName;
                Ext.MessageBox.hide();
            } else {
                Ext.MessageBox.hide();
                Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據,請先搜索查看!");
            }
        }
    });

}
onEditClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
    else if (row.length == 1) {
        EditFunction(row[0], VendorBrandStory);
    }
}
function VendorBrandPreview() {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
    else if (row.length == 1) {
        var brand_id = row[0].data.brand_id;
        Ext.Ajax.request({
            url: '/Vendor/VendorBrandPreview',
            params: { brand_id: brand_id },
            success: function (form, action) {
                var result = form.responseText;
                //var wl = "<a href=" + result + " target='new'>" + result + "</a>";
                if (result != "無預覽信息") {
                    window.open(result, '_blank');
                }
                else {
                    Ext.Msg.alert(INFORMATION, '對應品牌前台頁面不存在！');
                }
            }
        })
    }
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
