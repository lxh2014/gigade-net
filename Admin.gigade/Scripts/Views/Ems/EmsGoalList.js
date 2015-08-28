var pageSize = 25;


//model
Ext.define('gigade.EmsGoal', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },
        { name: "department_code", type: "string" },
          { name: "year", type: "int" },
          { name: "month", type: "int" },
        { name: "goal_amount", type: "int" },
        { name: "status", type: "int" },
          { name: "department_name", type: "string" },
         { name: "create_time", type: "string" },
         { name: "user_username", type: "string" }
    ]
});

var EmsGoalStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.EmsGoal',
    proxy: {
        type: 'ajax',
        url: '/Ems/GetEmsGoalList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});



EmsGoalStore.on('beforeload', function () {
    Ext.apply(EmsGoalStore.proxy.extraParams,
        {
            departgoal: Ext.getCmp('departgoal').getValue(),
          //  searchDategoal: Ext.getCmp('serchDategoal').getValue(),
            dategoal: Ext.getCmp('dategoal').getValue()
        })
});
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 0
});
//頁面載入
Ext.onReady(function () {
    var EmsGoal = Ext.create('Ext.grid.Panel', {
        id: 'EmsGoal',
        store: EmsGoalStore,
        plugins: [cellEditing],
        width: document.documentElement.clientWidth,
        columns: [
             { header: "編號", dataIndex: 'row_id', width: 60, align: 'center' },
             { header: '部門', dataIndex: 'department_name', width: 150, align: 'center' },
             { header: '年份', dataIndex: 'year', width: 100, align: 'center' },
             { header: "月份", dataIndex: 'month', width: 100, align: 'center' },
             { header: "目標", dataIndex: 'goal_amount', width: 100, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 0, allowDecimals: false } }
        , { header: '創建時間', dataIndex: 'create_time', width: 150, align: 'center' },
                 { header: '創建人', dataIndex: 'user_username', width: 100, align: 'center' },

        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
                           '->',
         {
             xtype: 'combobox', fieldLabel: "部門", id: 'departgoal', hidden: false, labelWidth: 50, store: EmsGoalComStore, displayField: 'department_name',editable:false,
             valueField: 'department_code',emptyText:'請選擇...',
         },
         //{
         //    xtype: 'combobox', fieldLabel: '查詢日期', id: 'serchDategoal', store: dateStore, value: 0, displayField: 'txt', labelWidth: 65, editable: false,
         //    valueField: 'value'
         //},
        { xtype: 'datetimefield', fieldLabel: '查詢日期', id: 'dategoal', hidden: false, value: new Date(), format: 'Y-m', editable: false,labelWidth:65,margin:'0 0 0 15' },
         { xtype: 'button', text: "查詢", id: 'query', hidden: false, handler: onQuery },
          {
              xtype: 'button', text: "重置", id: 'reset', hidden: false,
              handler: function () {
                  Ext.getCmp('departgoal').setValue('');
                 // Ext.getCmp('serchDategoal').setValue(0);
                  Ext.getCmp('dategoal').setValue(new Date());
              }
          },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EmsGoalStore,
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
                //如果編輯的是轉移數量
                var row_id = e.record.data.row_id;
                var EmsActual = e.field;
                var value = e.value;
                if (value == "") {
                    Ext.Msg.alert("提示信息", "目標不能為空!");
                }
                else {
                    if (e.field == "goal_amount") {
                        if (e.value != e.originalValue) {
                            Ext.Ajax.request({
                                url: '/Ems/EditEmsGoal',
                                params: {
                                    id: row_id,
                                    EmsActual: EmsActual,
                                    EmsValue: value
                                },
                                success: function (response) {
                                    var res = Ext.decode(response.responseText);
                                    if (res.success) {
                                        Ext.Msg.alert("提示信息", "保存成功!");
                                        EmsGoalStore.load();
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "保存失敗!");
                                        EmsGoalStore.load();
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [EmsGoal],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EmsGoal.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //ToolAuthority();
    //    QueryToolAuthorityByUrl('/EpaperContent/EpaperLogList');
   // EmsGoalStore.load({ params: { start: 0, limit: 25 } });
});

onAddClick = function () {
    editFunction(null, EmsGoalStore);
}

onQuery = function () {
    Ext.getCmp('EmsGoal').store.loadPage(1, {
        params: {
            departgoal: Ext.getCmp('departgoal').getValue(),
         //   searchDategoal: Ext.getCmp('serchDategoal').getValue(),
            dategoal: Ext.getCmp('dategoal').getValue()
        }
    });

}
