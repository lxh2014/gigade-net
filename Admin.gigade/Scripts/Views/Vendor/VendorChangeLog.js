
var pageSize = 25;
//表名數據
var searchTypeStore = Ext.create('Ext.data.Store', {
    fields: ['type', 'type_name'],
    autoLoad: true,
    data: [
       { type: '1', type_name: "供應商編號" },
       { type: '2', type_name: "供應商編碼" },
       { type: '3', type_name: "供應商名稱" }
    ]
});
//表名數據
var searchDateStore = Ext.create('Ext.data.Store', {
    fields: ['type', 'type_name'],
    autoLoad: true,
    data: [
       { type: '1', type_name: "建立時間" },
       { type: '2', type_name: "修改時間" }

    ] 
});

//歷史記錄列表grid
var listStore = Ext.create('Ext.data.Store', {
    fields: ['vendor_id', 'vendor_code', 'vendor_name_full', 'kuser_name', 'kdate', 'muser', 'muser_name', 'mdate'],
    proxy: {
        type: 'ajax',
        url: "/Vendor/VendorChangeList",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

listStore.on('beforeload', function () {
    Ext.apply(listStore.proxy.extraParams, {
        ven_type: Ext.getCmp('ven_type').getValue(),
        d_type: Ext.getCmp('d_type').getValue(),
        search_con: Ext.getCmp('search_con').getValue(),
        date_one: Ext.getCmp('date_one').getValue(),
        date_two: Ext.getCmp('date_two').getValue()

    });
});

//歷史詳情
var historyDetailStore = Ext.create('Ext.data.Store', {
    fields: ['vendor_id', 'vendor_name_full', 'tclModel'],
    proxy: {
        type: 'ajax',
        url: "/Vendor/VendorChangeDetailList",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
Ext.onReady(function () {
    Ext.create('Ext.Viewport', {
        id: "index",
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'north',//上北
            xtype: 'panel',
            height: 70,
            split: false,
            items: searchfrm,
            margins: '3 3 0 3'
        }, {
            region: 'west',//左西
            xtype: 'panel',
            margins: '3 0 3 3',
            width: 720,
            autoScroll: false
            ,
            items: gridlist
        }, {
            region: 'center',//中間
            xtype: 'panel',
            layout: 'fit',
            margins: '3 3 3 3'
            ,
            items: AuthView
        }],
        listeners: {
            resize: function () {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            }
        },
        renderTo: Ext.getBody()
    });

});
var AuthView = Ext.create('Ext.view.View', {
    deferInitialRefresh: false,
    autoScroll: true,
    frame: false,
    store: historyDetailStore,
    tpl: Ext.create('Ext.XTemplate',
        '<div id="div_historyDetail" class="View">',
        '<ul class="ul-detail">',
            '<tpl for=".">',
                '<li>',
                    '<h2>供應商編號：{vendor_id}   供應商名稱：{vendor_name_full}    </h2>',
                    '<table class="tbl-cls" style="width:800px">',
                    '<tr><th style="width:100px">欄位</th><th style="width:200px">欄位中文名稱</th><th style="width:200px">修改前</th><th style="200px">修改后</th></tr>',
                        '<tpl for="tclModel">',
                            '<tr ><td>{change_field}</td><td>{field_ch_name}</td><td>{old_value}</td><td>{new_value}</td>',
                        '</tr>',
                        '</tpl>',
                    '</table>',
                '</li>',
            '</tpl>',
        '</ul>',
        '</div>'
    ),
    itemSelector: 'li-detail'
});

var searchfrm = Ext.create('Ext.form.Panel', {
    id: 'searchfrm',
    layout: 'column',
    border: false,
    margins: '3 3 3 3',
    items: [
          {
              xtype: 'combobox',
              id: 'ven_type',
              name: 'ven_type',
              store: searchTypeStore,
              queryMode: 'local',
              width: 180,
             // value: '1',
              labelWidth: 60,
              margin: '5 10 0 5',
              displayField: 'type_name',
              valueField: 'type',
              editable: false,
              fieldLabel: "查詢條件",
              emptyText:'請選擇'
          },
          {
              xtype: 'textfield',
              width: 150,
              margin: '5 10 0 0',
              id: 'search_con',
              name: 'search_con',
              listeners: {
                  specialkey: function (field, e) {
                      if (e.getKey() == e.ENTER) {
                          Search();
                      }
                  },
                  focus: function () {
                      var searchType = Ext.getCmp("ven_type").getValue();
                      if (searchType == null || searchType == '') {
                          Ext.Msg.alert(INFORMATION, '請先選擇查詢條件');
                          this.focus = false;
                      }
                  }
              }
          }
          ,
           {
               xtype: 'combobox',
               id: 'd_type',
               name: 'd_type',
               store: searchDateStore,
               queryMode: 'local',
               width: 160,
               //value: '1',
               labelWidth: 60,
               margin: '5 10 0 5',
               displayField: 'type_name',
               valueField: 'type',
               editable: false,
               fieldLabel: "日期條件",
               emptyText: '請選擇'
           }
           ,
        {
            xtype: "datetimefield",
            width: 150,
            margin: '5 0 0 0',
            id: 'date_one',
            name: 'date_one',
            editable: false,
            format: 'Y-m-d H:i:s',
            time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
            submitValue: true,
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("date_one");
                    var end = Ext.getCmp("date_two");
                    if (end.getValue() == null) {
                        end.setValue(setNextMonth(start.getValue(), 1));
                    } else if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                        start.setValue(setNextMonth(end.getValue(), -1));
                    }
                },
                focus: function () {
                    var searchType = Ext.getCmp("d_type").getValue();
                    if (searchType == null || searchType == '') {
                        Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                        this.focus = false;
                    }
                }
            }
        },
          {
              xtype: 'displayfield',
              margin: '5 0 0 0',
              value: "~"
          },

     {
         xtype: "datetimefield",
         format: 'Y-m-d H:i:s',
         editable: false,
         time: { hour: 23, min: 59, sec: 59 },//開始時間00：00：00
         id: 'date_two',
         name: 'date_two',
         margin: '5 0 0 0',
         width: 150,
         listeners: {
             select: function (a, b, c) {
                 var start = Ext.getCmp("date_one");
                 var end = Ext.getCmp("date_two");
                 if (start.getValue() != "" && start.getValue() != null) {
                     if (end.getValue() < start.getValue()) {
                         Ext.Msg.alert(INFORMATION, DATA_TIP);
                         end.setValue(setNextMonth(start.getValue(), 1));
                     }
                 }
                 else {
                     start.setValue(setNextMonth(end.getValue(), -1));
                 }
             },
             focus: function () {
                 var searchType = Ext.getCmp("d_type").getValue();
                 if (searchType == null || searchType == '' ) {
                     Ext.Msg.alert(INFORMATION, '請先選擇日期條件');
                     this.focus = false;
                 }
             }
         }
     }

    ],
    buttonAlign: 'left',
    buttons: [
        {
            iconCls: 'icon-search',
            margin: '5 5 0 0',
            text: "查詢",
            handler: Search
        },
        {
            text: '重置',
            id: 'btn_reset',
            margin: '5 0 0 0',
            iconCls: 'ui-icon ui-icon-reset',
            handler: function () {
                this.up('form').getForm().reset();
            }
        },
         {
             icon: '../../../Content/img/icons/excel.gif',
             margin: '5 0 0 5',
             text: "匯出",
             handler: ExportCSV
         },
    ]

})

var gridlist = Ext.create('Ext.grid.Panel', {
    id: 'gridlist',
    autoScroll: true,
    store: listStore,
    height: document.documentElement.clientHeight - 74,
    border: false,
    columns: [{ header: '供應商編號', dataIndex: 'vendor_id', align: 'center', width: 80 },
        { header: '供應商編碼', dataIndex: 'vendor_code', align: 'center', width: 80 },
        { header: '供應商名稱', dataIndex: 'vendor_name_full', align: 'center', flex: 1 },
          { header: '建立人', dataIndex: 'kuser_name', align: 'center', width: 65 },
        { header: '建立時間', dataIndex: 'kdate', align: 'center', width: 130 },
          { header: '修改人', dataIndex: 'muser_name', align: 'center', width: 65 },
        { header: '修改時間', dataIndex: 'mdate', align: 'center', width: 130 }],
    bbar: Ext.create('Ext.PagingToolbar', {
        store: listStore,
        pageSize: pageSize,
        displayInfo: true,
        displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
        emptyMsg: NOTHING_DISPLAY
    }),
    listeners: {
        itemclick: function (grid, record) {
            historyDetailStore.removeAll();
            historyDetailStore.load({
                params: {
                    vendor_id: record.data.vendor_id,
                    muser: record.data.muser,
                    mdate: record.data.mdate
                }
            })
        }

    }

})

 
//查詢
function Search() {
    var searchType = Ext.getCmp("d_type").getValue();
    if ((searchType == null || searchType == '') && (Ext.getCmp('date_one').getValue() != null && Ext.getCmp('date_two').getValue() != null)) {
        Ext.Msg.alert(INFORMATION, '請先選擇日期條件');      
    }
    else if (Ext.getCmp('search_con').getValue() != "" || (Ext.getCmp('date_one').getValue() != null && Ext.getCmp('date_two').getValue() != null)) {
        listStore.removeAll();
        listStore.loadPage(1, {
            callback: function () {
                if (listStore.data.items.length == 0) {
                    Ext.Msg.alert(INFORMATION, "該條件下沒有供應商異動記錄");
                }
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}

//匯出
function ExportCSV() {
    Ext.Ajax.request({
        url: "/Vendor/VendorLogExport",
        params: {
            ven_type: Ext.getCmp('ven_type').getValue(),
            d_type: Ext.getCmp('d_type').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            date_one: Ext.getCmp('date_one').getValue(),
            date_two: Ext.getCmp('date_two').getValue()
        },
        success: function (response) {
            if (response.responseText.split(',')[0] == "true") {
                window.location.href = '../..' + response.responseText.split(',')[2] + response.responseText.split(',')[1];
            }
        }
    });
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

