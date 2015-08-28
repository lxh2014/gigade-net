var pageSize = 25;
/*************站臺管理主頁面****************/
var yunsongtype = Ext.create('Ext.data.Store', {
    fields: ['id', 'name'],
    data: [
        { "id": "2", "name": "常溫" },
        { "id": "92", "name": "冷凍" }
        //...
    ]
});
Ext.define("gigade.delivertype", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});
var delivertype = Ext.create('Ext.data.Store', {
    model: 'gigade.delivertype',
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetAllkindType",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    },
    autoLoad: false
});
//Model
Ext.define('gigade.Ilocs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "count", type: "int" },
        { name: "dcount", type: "int" },
        { name: "Order_Id", type: "int" },
        { name: "deliver_store_name", type: "string" },
        { name: "deliver_code", type: "string" },
        { name: "deliver_id", type: "string" }
    ]
});

var AseidStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ilocs',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetAseldList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    , autoLoad: false
});

AseidStore.on('beforeload', function () {
    Ext.apply(AseidStore.proxy.extraParams,
        {
            type_id: Ext.getCmp('search').getValue(),
            deliver_type: Ext.getCmp('deliver_type').getValue(),
            beforeradio: Ext.getCmp('radio1').getValue()
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdAseid").down('#addjob').setDisabled(selections.length == 0);
            var row = Ext.getCmp("gdAseid").getSelectionModel().getSelection();
            if (row == 0)//解決當什麼都不選時,輸入框類有兩個很大的值
            {
                Ext.getCmp("jicang").setValue(null);
                Ext.getCmp("diaodu").setValue(null);
            }
            if (Ext.getCmp('searchcontent').getValue() == null)//判斷如果裡面輸入有數字表示已觸發事件
            {
                var recordsOrderIdChecked = new Array();//訂單編號
                var recordsCountChecked = new Array();//寄倉
                var recordsDCountChecked = new Array();//調度
                var row = Ext.getCmp("gdAseid").getSelectionModel().getSelection();
                var rowOrderIDs = "";
                for (var i = 0; i < row.length; i++) {
                    rowOrderIDs += row[i].data.Order_Id + ',';
                }//獲取到本頁面所有的訂單
                var jicangIDs = "";
                for (var i = 0; i < row.length; i++) {
                    jicangIDs += row[i].data.count + ',';
                }//獲取到本頁面所有的count
                var diaoduIDs = "";
                for (var i = 0; i < row.length; i++) {
                    diaoduIDs += row[i].data.dcount + ',';
                }//獲取到本頁面所有的dcount
                recordsOrderIdChecked = rowOrderIDs.split(",")

                recordsCountChecked = jicangIDs.split(",")

                recordsDCountChecked = diaoduIDs.split(",")

                var jicang = 0;
                var diaodu = 0;
                for (j = 0; j < row.length; j++) {
                    jicang = parseInt(recordsCountChecked[j]) + jicang;
                    diaodu = parseInt(recordsDCountChecked[j]) + diaodu;
                }
                if (jicang > 0) {
                    Ext.getCmp("jicang").setValue(jicang);

                }
                if (diaodu > 0) {
                    Ext.getCmp("diaodu").setValue(diaodu);

                }
            }

        }
    }
});

Ext.onReady(function () {

    var gdAseid = Ext.create('Ext.grid.Panel', {
        id: 'gdAseid',
        store: AseidStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "訂單編號", dataIndex: 'Order_Id', width: 150, align: 'center' },
            { header: "寄倉品類數量", dataIndex: 'count', width: 150, align: 'center' },
            { header: "調度品類數量", dataIndex: 'dcount', width: 150, align: 'center' },
            { header: "出貨編號", dataIndex: 'deliver_code', width: 150, align: 'center' },
            { header: "物流方式", dataIndex: 'deliver_store_name', width: 150, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: "生產工作項", id: 'addjob', hidden: false, disabled: true, iconCls: 'icon-user-add', handler: GetJobMsg }

        ],
        dockedItems: [
          {   //類似于tbar
              xtype: 'toolbar',
              dock: 'top',
              items: [

                 {
                     xtype: 'combobox',
                     name: 'search',
                     id: 'search',
                     editable: false,
                     fieldLabel: "請選擇運送方式",
                     emptyText: "請選擇",
                     labelWidth: 90,
                     store: yunsongtype,
                     queryMode: 'local',
                     submitValue: true,
                     displayField: 'name',
                     valueField: 'id',
                     typeAhead: true,
                     forceSelection: false,
                     listeners: {//再不選擇的時候,不會出現數據。
                         select: function () {
                             var type_id = Ext.getCmp('search').getValue();
                             var type = 0;
                             var this_deliver_type = 0;
                             if (Ext.getCmp('deliver_type').getValue() == null)//表示未選擇出貨方式 常溫 或者 冷凍
                             {
                                 this_deliver_type = 0;
                             }
                             else {
                                 this_deliver_type = Ext.getCmp('deliver_type').getValue();
                             }
                             if (Ext.getCmp('radio2').getValue() == true) {
                                 type = 2;
                             }
                             else if (Ext.getCmp('radio1').getValue() == true) {
                                 type = 1;
                             }
                             if (type_id == "2") {//當選擇常溫時

                                 Ext.Ajax.request({
                                     url: "/WareHouse/GetAseldList",
                                     method: 'post',
                                     type: 'text',
                                     params: {
                                         type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                         deliver_type: this_deliver_type,
                                         radio: type
                                     },
                                     success: function (form, action) {
                                         var result = Ext.decode(form.responseText);
                                         if (result.success) {
                                             AseidStore.loadPage(1, {
                                                 params: {
                                                     type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                                     deliver_type: this_deliver_type,
                                                     radio: type
                                                 }
                                             });
                                         }
                                     }
                                 });
                             }
                             else if (type_id = "92") {//當選擇冷凍時
                                 Ext.Ajax.request({
                                     url: "/WareHouse/GetAseldList",
                                     method: 'post',
                                     type: 'text',
                                     params: {
                                         type_id: Ext.getCmp('search').getValue(),
                                         deliver_type: this_deliver_type,
                                         radio: type
                                     },
                                     success: function (form, action) {
                                         var result = Ext.decode(form.responseText);
                                         if (result.success) {
                                             AseidStore.loadPage(1, {
                                                 params: {
                                                     type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                                     deliver_type: this_deliver_type,
                                                     radio: type
                                                 }
                                             });
                                         }
                                         //if (result.success) {
                                         //    AseidStore.loadPage(1);

                                         //}
                                     }
                                 });
                             }
                         }
                     }
                 },
                  { xtype: 'label', id: 'changwenorlengcang', text: "（選擇常溫/冷藏后會出現各自的數據）", style: 'color:red' },
                   { xtype: 'displayfield', margin: '0 0 0 10', value: "是否選擇調度:" },
                   {
                       xtype: 'radiofield',
                       boxLabel: '有調度',
                       name: 'diaodu',
                       id: 'radio2',
                       checked: true,
                       listeners: {//再不選擇的時候,不會出現數據。
                           change: function () {
                               //alert(Ext.getCmp('radio2').getValue());
                               if (Ext.getCmp('radio2').getValue() == true) {
                                   if (Ext.getCmp('search').getValue() == null)//表示未選擇出貨方式 常溫 或者 冷凍
                                   {
                                       return;
                                   }
                                   else if (Ext.getCmp('search').getValue() == "2" || Ext.getCmp('search').getValue() == "92") {
                                       var type_id = Ext.getCmp('search').getValue();
                                       Ext.Ajax.request({
                                           url: "/WareHouse/GetAseldList",
                                           method: 'post',
                                           type: 'text',
                                           params: {
                                               type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                               radio: 2,
                                               deliver_type: Ext.getCmp('deliver_type').getValue()
                                           },
                                           success: function (form, action) {
                                               var result = Ext.decode(form.responseText);
                                               if (result.success) {
                                                   AseidStore.loadPage(1, {
                                                       params: {
                                                           type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                                           radio: 2,
                                                           deliver_type: Ext.getCmp('deliver_type').getValue()
                                                       }
                                                   });
                                               }
                                           }
                                       });
                                   }
                               }

                           }
                       }
                   },
                      {
                          xtype: 'radiofield',
                          boxLabel: '無調度',
                          name: 'diaodu',
                          id: 'radio1',
                          listeners: {//再不選擇的時候,不會出現數據。
                              change: function () {
                                  //alert(Ext.getCmp('radio1').getValue());
                                  if (Ext.getCmp('radio1').getValue() == true) {
                                      if (Ext.getCmp('search').getValue() == null)//表示未選擇出貨方式 常溫 或者 冷凍
                                      {
                                          return;
                                      }
                                      else if (Ext.getCmp('search').getValue() == "2" || Ext.getCmp('search').getValue() == "92") {
                                          var type_id = Ext.getCmp('search').getValue();
                                          Ext.Ajax.request({
                                              url: "/WareHouse/GetAseldList",
                                              method: 'post',
                                              type: 'text',
                                              params: {
                                                  type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                                  radio: 1,
                                                  deliver_type: Ext.getCmp('deliver_type').getValue()
                                              },
                                              success: function (form, action) {
                                                  var result = Ext.decode(form.responseText);
                                                  if (result.success) {
                                                      AseidStore.loadPage(1, {
                                                          params: {
                                                              type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                                              radio: 1,
                                                              deliver_type: Ext.getCmp('deliver_type').getValue()
                                                          }
                                                      });
                                                  }

                                              }
                                          });
                                      }
                                  }

                                  //Ext.Ajax.request({
                                  //    url: "/WareHouse/GetAseldList",
                                  //    method: 'post',
                                  //    type: 'text',
                                  //    params: {
                                  //        type_id: Ext.getCmp('search').getValue()//傳值運送方式 2 表示 常溫 92 表示冷凍
                                  //    },
                                  //    success: function (form, action) {
                                  //        var result = Ext.decode(form.responseText);
                                  //        AseidStore.loadPage(1);
                                  //        if (result.success) {
                                  //        }
                                  //    }
                                  //});
                              }
                          }
                      },
               {//運送方式
                   xtype: 'combobox',
                   name: 'deliver_type',
                   id: 'deliver_type',
                   editable: false,
                   fieldLabel: "請選擇物流方式",
                   emptyText: "請選擇",
                   labelWidth: 90,
                   margin: '0 0 0 10',
                   store: delivertype,
                   queryMode: 'local',
                   submitValue: true,
                   displayField: 'parameterName',
                   valueField: 'ParameterCode',
                   typeAhead: true,
                   forceSelection: false,
                   listeners: {//再不選擇的時候,不會出現數據。
                       beforerender: function () {
                           delivertype.load({
                               callback: function () {
                                   delivertype.insert(0, { ParameterCode: '0', parameterName: '全部' });
                                   Ext.getCmp("deliver_type").setValue(delivertype.data.items[0].data.ParameterCode);
                               }
                           });
                       },
                       select: function () {
                           if (Ext.getCmp('search').getValue() == null)//表示未選擇出貨方式 常溫 或者 冷凍
                           {
                               return;
                           }
                           else if (Ext.getCmp('search').getValue() == "2" || Ext.getCmp('search').getValue() == "92") {
                               var type_id = Ext.getCmp('search').getValue();
                               var type = 0;
                               if (Ext.getCmp('radio2').getValue() == true) {
                                   type = 2;
                               }
                               else if (Ext.getCmp('radio1').getValue() == true) {
                                   type = 1;
                               }
                               Ext.Ajax.request({
                                   url: "/WareHouse/GetAseldList",
                                   method: 'post',
                                   type: 'text',
                                   params: {
                                       type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                       radio: type,
                                       deliver_type: Ext.getCmp('deliver_type').getValue()
                                   },
                                   success: function (form, action) {
                                       var result = Ext.decode(form.responseText);
                                       if (result.success) {
                                           AseidStore.loadPage(1, {
                                               params: {
                                                   type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                                   deliver_type: Ext.getCmp('deliver_type').getValue(),
                                                   radio: type
                                               }
                                           });
                                       }
                                   }
                               });
                           }
                       }
                   }
               },


                  '->',
                  { xtype: 'displayfield', value: "總計:" },
                  { xtype: 'displayfield', fieldLabel: "寄倉", id: 'jicang', width: 100, labelWidth: 55, border: false, fieldStyle: 'border:0px;color:red;', style: 'color:red' },
                  { xtype: 'displayfield', fieldLabel: "調度", id: 'diaodu', width: 100, border: false, fieldStyle: 'border:0px;color:red;', labelWidth: 55, style: 'color:red' },

                  {
                      xtype: 'numberfield', allowBlank: true, margin: '0 0 0 10', fieldLabel: '請輸入值', minValue: 0, id: 'searchcontent', name: 'searchcontent', labelWidth: 80
                      //,
                      //listeners: {//再不選擇的時候,不會出現數據。
                      //    blur: function () {
                      //        QueryChecks();
                      //    }
                      //}
                  },
           {
               xtype: 'button',
               text: "確定",
               id: 'btnQueryChecks',
               handler: QueryChecks
           },
            {
                xtype: 'button',
                text: "重置",
                id: 'btnreset',
                handler: Reset
            }
              ]
          }],
        bbar: Ext.create('Ext.PagingToolbar', {
            id: 'pageToolBar',
            store: AseidStore,
            pageSize: pageSize,
            autoDestroy: true,
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
        items: [gdAseid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdAseid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    AseidStore.load({ params: { start: 0, limit: 25 } });
});

function QueryChecks(x) {//選擇多少個
    var results = Ext.getCmp("gdAseid").getSelectionModel();//
    results.selectAll();
    var recordsOrderIdChecked = new Array();//訂單編號
    var recordsCountChecked = new Array();//寄倉
    var recordsDCountChecked = new Array();//調度
    var row = Ext.getCmp("gdAseid").getSelectionModel().getSelection();
    var rowOrderIDs = "";
    for (var i = 0; i < row.length; i++) {
        rowOrderIDs += row[i].data.Order_Id + ',';
    }//獲取到本頁面所有的訂單
    var jicangIDs = "";
    for (var i = 0; i < row.length; i++) {
        jicangIDs += row[i].data.count + ',';
    }//獲取到本頁面所有的count
    var diaoduIDs = "";
    for (var i = 0; i < row.length; i++) {
        diaoduIDs += row[i].data.dcount + ',';
    }//獲取到本頁面所有的dcount
    recordsOrderIdChecked = rowOrderIDs.split(",")

    recordsCountChecked = jicangIDs.split(",")

    recordsDCountChecked = diaoduIDs.split(",")

    var all = Ext.getCmp("searchcontent").getValue();
    var checks = 0;
    results.deselectAll();
    var jicang = 0;
    var diaodu = 0;
    for (j = 0; j < row.length; j++) {
        checks = checks + parseInt(recordsCountChecked[j]) + parseInt(recordsDCountChecked[j]);
        if (all != null) {
            if (parseInt(all) >= checks) {
                jicang = parseInt(recordsCountChecked[j]) + jicang;
                diaodu = parseInt(recordsDCountChecked[j]) + diaodu;
                results.selectRange(0, j);
            }
        }
    }
    if (jicang > 0) {
        Ext.getCmp("jicang").setValue(jicang);
    }
    if (diaodu > 0) {
        Ext.getCmp("diaodu").setValue(diaodu);
    }
    Ext.getCmp('searchcontent').setValue(null);
}

function Reset() {
    var results = Ext.getCmp("gdAseid").getSelectionModel();//重置
    results.deselectAll();
    Ext.getCmp("jicang").setValue(null);
    Ext.getCmp("diaodu").setValue(null);
}
//生產工作單
GetJobMsg = function () {
    var row = Ext.getCmp("gdAseid").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("是否生產工作單", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.deliver_id + ',';
                    // alert(rowIDs);
                }
                var type = 0;
                if (Ext.getCmp('radio2').getValue() == true) {
                    type = 2;
                }
                else if (Ext.getCmp('radio1').getValue() == true) {
                    type = 1;
                }

                var this_deliver_type = 0;
                if (Ext.getCmp('deliver_type').getValue() == null)//表示未選擇出貨方式 常溫 或者 冷凍
                {
                    this_deliver_type = 0;
                }
                else {
                    this_deliver_type = Ext.getCmp('deliver_type').getValue();
                }
                Ext.Ajax.request({
                    url: '/WareHouse/CreateTallyList',//執行方法
                    method: 'post',
                    params: {
                        deliver_id: rowIDs,
                        type_id: Ext.getCmp('search').getValue(),
                        radio: type
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.assg != null && result.assg != "") {
                                Ext.Msg.alert(INFORMATION, "生產工作單成功!工作單號為:" + result.assg);
                            }
                            AseidStore.loadPage(1, {
                                params: {
                                    type_id: Ext.getCmp('search').getValue(),//傳值運送方式 2 表示 常溫 92 表示冷凍
                                    deliver_type: this_deliver_type,
                                    radio: type
                                }
                            });

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





