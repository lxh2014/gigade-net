var pageSize = 25;

Ext.define('gigade.EmsDepRe', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "relation_id", type: "int" },
        { name: "relation_type", type: "int" },
        { name: "relation_order_count", type: "int" },
        { name: "relation_order_cost", type: "int" },
        { name: "relation_dep", type: "int" },
        { name: "update_time", type: "string" },
        { name: "create_time", type: "string" },
        { name: "relation_create_type", type: "int" },
        { name: "create_user", type: "int" },
        { name: "update_user", type: "int" },
        { name: "relation_year", type: "int" },
        { name: "relation_month", type: "int" },
        { name: "relation_day", type: "int" },
        { name: "dep_name", type: "string" },
        { name: "user_username", type: "string" },
    ]
});

var EmsDepReStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.EmsDepRe',
    proxy: {
        type: 'ajax',
        url: '/EmsDepRelation/EmsDepRelationList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});



EmsDepReStore.on('beforeload', function () {
    Ext.apply(EmsDepReStore.proxy.extraParams,
        {
            dep_code: Ext.getCmp('depart').getValue(),
            datatype: Ext.getCmp('datatype').getValue(),
          //  serchDate: Ext.getCmp('serchDate').getValue(),
            date: Ext.getCmp('date').getValue(),
            relation_type: Ext.getCmp('re_type').getValue(),
        });
});
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 2
});
var type = 2;
//頁面載入
Ext.onReady(function () {
    var EmsDepRe = Ext.create('Ext.grid.Panel', {
        id: 'EmsDepRe',
        store: EmsDepReStore,
        plugins: [cellEditing],
        width: document.documentElement.clientWidth,
        columns: [
             { header: "編號", dataIndex: 'relation_id', width: 60, align: 'center' },
             { header: '部門', dataIndex: 'dep_name', width: 150, align: 'center' },
              {
                  header: '公關單類型', dataIndex: 'relation_type', width: 100, align: 'center', renderer: function (value) {
                      if (value == 1) {
                          return "公關單";
                      }
                      else if (value == 2) {
                          return "報廢單";
                      }
                  }
                          },
             { header: '年', dataIndex: 'relation_year', width: 100, align: 'center' },
             { header: "月", dataIndex: 'relation_month', width: 100, align: 'center' },
             { header: "日", dataIndex: 'relation_day', width: 100, align: 'center' },
              { header: "訂單成本", dataIndex: 'relation_order_cost', width: 100, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } },
               { header: "訂單筆數", dataIndex: 'relation_order_count', width: 100, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } },
              {
                  header: '類型', dataIndex: 'relation_create_type', width: 100, align: 'center', renderer: function (value) {
                      if (value == 1) {
                          return "系統生成";
                      }
                      else if (value == 2) {
                          return "人工keyin";
                      }
                  }
              },
               { header: '創建時間', dataIndex: 'create_time', width: 150, align: 'center' },
                 { header: '創建人', dataIndex: 'user_username', width: 100, align: 'center' },

        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
        '->',
         {
             xtype: 'combobox', fieldLabel: "部門", id: 'depart', hidden: false, labelWidth: 50, store: EmsDepStore, displayField: 'dep_name',
             valueField: 'dep_code', emptyText: '請選擇...',editable:false,
         },
           {
               xtype: 'combobox', fieldLabel: "公關單類型", id: 're_type', hidden: false, labelWidth: 80, store: reTypeStore, displayField: 'txt',
               valueField: 'value', emptyText: '請選擇...', margin: '0 0 0 5', editable: false,
           },
         {
             xtype: 'combobox', fieldLabel: "類型", id: 'datatype', labelWidth: 50, store: typeStore, displayField: 'datatxt', valueField: 'datavalue',
             value: '0', margin: '0 0 0 5', editable: false,value:'2',
         },
         //{
         //    xtype: 'combobox', fieldLabel: '查詢日期', id: 'serchDate', store: dateStore, value: 0, displayField: 'txt', labelWidth: 65,
         //    valueField: 'value', margin: '0 0 0 5', editable: false,
         //},
        { xtype: 'datetimefield', id: 'date', fieldLabel: '查詢日期', hidden: false, value: new Date(), format: 'Y-m-d', editable: false,labelWidth:60,margin:'0 0 0 10' },
         { xtype: 'button', text: "查詢", id: 'query', hidden: false, handler: onQuery, iconCls: 'icon-search', },
          {
              xtype: 'button', text: "重置", id: 'reset', hidden: false, iconCls: 'ui-icon ui-icon-reset',
              handler: function () {
                  Ext.getCmp('depart').setValue('');
               //   Ext.getCmp('serchDate').setValue(0);
                  Ext.getCmp('date').setValue(new Date());
                  Ext.getCmp('datatype').setValue('2');
                  Ext.getCmp('re_type').setValue('0');
                  
              }
          },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EmsDepReStore,
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
            },
            edit: function (editor, e) {
                if (e.record.data.relation_create_type == 1) {
                    Ext.Msg.alert("提示信息", "此爲系統自動生成,所做更改無效！");
                    EmsDepReStore.load();
                }
                else {
                    //如果編輯的是轉移數量
                    var relation_id = e.record.data.relation_id;
                    var EmsDep = e.field;
                    var value = e.value;
                    if (e.field == "relation_order_cost" || e.field == "relation_order_count") {
                        if (e.value != e.originalValue) {
                            Ext.Ajax.request({
                                url: '/EmsDepRelation/EditEmsDepR',
                                params: {
                                    relation_id: relation_id,
                                    emsDep: EmsDep,
                                    value: value
                                },
                                success: function (response) {
                                    var res = Ext.decode(response.responseText);
                                    if (res.success) {
                                        Ext.Msg.alert("提示信息", "保存成功!");
                                        EmsDepReStore.load();
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "保存失敗!");
                                        EmsDepReStore.load();
                                    }
                                }
                            });

                        }
                    }
                }
            }
        },
        viewConfig: {
            forceFit: true,
            getRowClass: function (record, rowIndex, rowParams, store) {
                if (record.data.relation_create_type == 1) {
                    return 'ems_actual_type';//注意这里返回的是定义好的css类；列如：(.ppp_ddd_sss div{background-color:red})定义到你页面访问到的css文件里。
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [EmsDepRe],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EmsDepRe.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
 //   EmsDepReStore.load({ params: { start: 0, limit: 25 } });
});

onAddClick = function () {
    editFunction(null, EmsDepReStore);
}
onQuery = function () {
    Ext.getCmp('EmsDepRe').store.loadPage(1, {
        params: {
            dep_code: Ext.getCmp('depart').getValue(),
            datatype: Ext.getCmp('datatype').getValue(),
         //   serchDate: Ext.getCmp('serchDate').getValue(),
            date: Ext.getCmp('date').getValue(),
            relation_type: Ext.getCmp('re_type').getValue(),
        }
    });

}



