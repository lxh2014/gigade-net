var pageSize = 25;

Ext.define('gigade.EmsActual', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },
        { name: "department_code", type: "string" },
                { name: "department_name", type: "string" },
          { name: "year", type: "int" },
          { name: "month", type: "int" },
              { name: "day", type: "int" },
      { name: "type", type: "int" },
        { name: "cost_sum", type: "int" },
         { name: "order_count", type: "int" },
           { name: "amount_sum", type: "int" },
        { name: "status", type: "int" },
          { name: "iskeyin", type: "int" },
              { name: "create_time", type: "string" },
                  { name: "user_username", type: "string" }
    ]
});

var EmsActualStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.EmsActual',
    proxy: {
        type: 'ajax',
        url: '/Ems/GetEmsActualList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});



EmsActualStore.on('beforeload', function () {
    Ext.apply(EmsActualStore.proxy.extraParams,
        {
            departactual: Ext.getCmp('departactual').getValue(),
            dateactual: Ext.getCmp('dateactual').getValue(),
            datatype: Ext.getCmp('datatype').getValue()
        });
});
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 0
});
var type = 2;
//頁面載入
Ext.onReady(function () {
    var EmsActual = Ext.create('Ext.grid.Panel', {
        id: 'EmsActual',
        store: EmsActualStore,
        plugins: [cellEditing],
        width: document.documentElement.clientWidth,
        columns: [
             { header: "編號", dataIndex: 'row_id', width: 60, align: 'center' },
             { header: '部門', dataIndex: 'department_name', width: 150, align: 'center' },
             { header: '年', dataIndex: 'year', width: 100, align: 'center' },
             { header: "月", dataIndex: 'month', width: 100, align: 'center' },
             { header: "日", dataIndex: 'day', width: 100, align: 'center' },
              { header: "成本", dataIndex: 'cost_sum', width: 100, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } },
               { header: "訂單總數", dataIndex: 'order_count', width: 100, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } },
               { header: "累計實績", dataIndex: 'amount_sum', width: 100, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } },
              {
                  header: '類型', dataIndex: 'type', width: 100, align: 'center', renderer: function (value) {
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

                               //   { header: '類型', dataIndex: 'type', width: 100, align: 'center' },

        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
        '->',
         {
             xtype: 'combobox', fieldLabel: "部門", id: 'departactual', hidden: false, labelWidth: 50, store: EmsGoalComStore, displayField: 'department_name',
             valueField: 'department_code', emptyText: '請選擇...',editable:false,
         },
         {
             xtype: 'combobox', fieldLabel: "類型", id: 'datatype', labelWidth: 50, store: typeStore, displayField: 'datatxt', valueField: 'datavalue', editable: false,
             value:'2'
         },
        { xtype: 'datetimefield', id: 'dateactual', hidden: false, value: new Date(), format: 'Y-m-d', fieldLabel: '查詢日期', editable: false, },
         { xtype: 'button', text: "查詢", id: 'query', hidden: false, handler: onQueryActual },
          {
              xtype: 'button', text: "重置", id: 'reset', hidden: false,
              handler: function () {
                  Ext.getCmp('departactual').setValue('');
                  Ext.getCmp('dateactual').setValue(new Date());
                  Ext.getCmp('datatype').setValue('2');
                  
              }
          },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EmsActualStore,
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
                if (e.record.data.type == 1) {
                    //e.originalValue = e.originalValue;
                    Ext.Msg.alert("提示信息", "此爲系統自動生成,所做更改無效！");
                    EmsActualStore.load();
                }
                else {
                    //如果編輯的是轉移數量
                    var row_id = e.record.data.row_id;
                    var EmsActual = e.field;
                    var value = e.value;
                    if (e.field == "amount_sum" || e.field == "order_count" || e.field == "cost_sum") {
                        if (e.value != e.originalValue) {
                            Ext.Ajax.request({
                                url: '/Ems/EditEmsActual',
                                params: {
                                    id: row_id,
                                    EmsActual: EmsActual,
                                    EmsValue: value
                                },
                                success: function (response) {
                                    var res = Ext.decode(response.responseText);
                                    if (res.success) {
                                        Ext.Msg.alert("提示信息", "保存成功!");
                                        EmsActualStore.load();
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "保存失敗!");
                                        EmsActualStore.load();
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
                if (record.data.type ==1) {
                    return 'ems_actual_type';//注意这里返回的是定义好的css类；列如：(.ppp_ddd_sss div{background-color:red})定义到你页面访问到的css文件里。
                }
            }
        }
    });

Ext.create('Ext.container.Viewport', {
    layout: 'fit',
    items: [EmsActual],
    renderTo: Ext.getBody(),
    autoScroll: true,
    listeners: {
        resize: function () {
            EmsActual.width = document.documentElement.clientWidth;
            this.doLayout();
        }
    }
});
//EmsActualStore.load({ params: { start: 0, limit: 25 } });
});

onAddClick = function () {
    editFunction(null, EmsActualStore);
}
onQueryActual = function () {
    Ext.getCmp('EmsActual').store.loadPage(1, {
        params: {
            departactual: Ext.getCmp('departactual').getValue(),
            dateactual: Ext.getCmp('dateactual').getValue(),
            datatype: Ext.getCmp('datatype').getValue()
        }
    });

}



