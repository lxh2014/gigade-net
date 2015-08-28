editSpecFunction = function (vendor_id, product_id) {
    var pageSize = 25;
    Ext.define('gigade.VendorSpecProduct', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "product_id", type: "int" },
            { name: "item_id", type: "int" },
            { name: "specone", type: "string" },
            { name: "spectwo", type: "string" },
            { name: "item_stock", type: "int" },
            { name: "spec_status_one", type: "string" },
            { name: "spec_status_two", type: "string" },
            { name: "spec_id_one", type: "string" },
            { name: "spec_id_two", type: "string" }


        ]
    });
    var VendorProductSpecStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        pageSize: pageSize,
        model: 'gigade.VendorSpecProduct',
        proxy: {
            type: 'ajax',
            url: '/VendorStock/GetVendorProductSpec',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
        //    autoLoad: true
    });
    var ThisTypeStore = new Ext.data.SimpleStore({
        fields: ['value', 'name'],
        data: [
            ['1', '顯示'],
            ['0', '隱藏']
        ]
    });
    var ThisTypeStoreTwo = new Ext.data.SimpleStore({
        fields: ['value', 'name'],
        data: [
            ['1', '顯示'],
            ['0', '隱藏']
        ]
    });
    VendorProductSpecStore.on('beforeload', function () {
        Ext.apply(VendorProductSpecStore.proxy.extraParams,
            {
                product_id: product_id
            });
    });
    VendorProductSpecStore.load();
    var gdSpecMeg = Ext.create('Ext.grid.Panel', {
        id: 'gdSpecMeg',
        title: "",
        store: VendorProductSpecStore,
        sortableColumns: false,
        columnLines: true,
        hidden: false,
        frame: true,
        width: 650,
        height: 450,
        columns: [
            { header: PRODUCTID, dataIndex: 'product_id', width: 150, hidden: true, align: 'center' },
            { header: ITEMID, dataIndex: 'item_id', width: 150, align: 'center' },
            {
                header: SPECONE, dataIndex: 'specone', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != "" && value != null) {
                        if (record.data.spec_status_one == 1) {
                            return value;
                        }
                        else {
                            return "<span style='color:red;'border='1px'>" + value + "</span>";
                        }
                    }
                    else {
                        return "";
                    }
                }
            },
             {
                 header: "顯示/隱藏", dataIndex: 'spec_status_one', width: 70, align: 'center',
                 editor: {
                     xtype: 'combobox',
                     id: 'this_type_one',
                     name: 'this_type_one',
                     store: ThisTypeStore,
                     hideLabel: true,
                     lazyRender: true, //值为true时阻止ComboBox渲染直到该对象被请求
                     displayField: "name",
                     valueField: "value",
                     mode: "local",
                     editable: false,
                     triggerAction: "all"
                 },
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (record.data.specone != "" && record.data.specone != null) {
                         if (value == 1) {
                             return "顯示";
                         }
                         else {
                             return "<span style='color:red;'border='1px'>隱藏</span>";
                         }
                     }
                     else {
                         return "";
                     }
                 }
             },
              {
                  header: SPECTWO, dataIndex: 'spectwo', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      if (value != "" && value != null) {
                          if (record.data.spec_status_two == 1) {
                              return value;
                          }
                          else {
                              return "<span style='color:red;'border='1px'>" + value + "</span>";
                          }
                      }
                      else {
                          return "";
                      }
                  }
              },
           {
               header: "顯示/隱藏", dataIndex: 'spec_status_two', width: 70, align: 'center',
               editor: {
                   xtype: 'combobox',
                   id: 'this_type_two',
                   name: 'this_type_two',
                   store: ThisTypeStoreTwo,
                   hideLabel: true,
                   lazyRender: true, //值为true时阻止ComboBox渲染直到该对象被请求
                   displayField: "name",
                   valueField: "value",
                   mode: "local",
                   editable: false,
                   triggerAction: "all"
               },
               renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                   if (record.data.spectwo != "" && record.data.spectwo != null) {
                       if (value == 1) {
                           return "顯示";
                       }
                       else {
                           return "<span style='color:red;'border='1px'>隱藏</span>";
                       }
                   }
                   else {
                       return "";
                   }
               }
           },
            {
                header: STOCK, dataIndex: 'item_stock', width: 120, align: 'center', editor: {
                    xtype: 'numberfield',
                    format: 'Y-m-d',
                    allowBlank: false,
                    minValue: 0
                }
            },
        ],
        selType: 'cellmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        viewConfig: {
            emptyText: '<span>' + NOMESSAGE + '</span>'
        },
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            },
            edit: function (editor, e) {
                //如果編輯的是庫存數量
                if (e.field == "item_stock") {
                    //如果轉移數量不為零的話
                    if (e.value != e.originalValue) {
                        Ext.Ajax.request({
                            url: "/VendorStock/UpdateStock",
                            method: 'post',
                            type: 'text',
                            params: {
                                newstock: e.value,
                                item_id: e.record.data.item_id,
                                oldstock: e.originalValue,
                                vendor_id: vendor_id
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, STOCKUPDATESUCCESS);
                                    VendorProductSpecStore.load();
                                } else {
                                    Ext.Msg.alert(INFORMATION, STOCKUPDATEFAIL);
                                    VendorProductSpecStore.load();
                                }

                            },
                            failure: function (form, action) {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                    else {
                        VendorProductSpecStore.load();
                    }
                }

                if (e.field == "spec_status_one") {
                    //如果轉移數量不為零的話
                    if (e.originalValue != "" && e.originalValue != null) {
                        if (e.value != e.originalValue) 
                        {
                            Ext.Ajax.request({
                                url: "/VendorStock/UpdateStatus",//顯示或者隱藏
                                method: 'post',
                                type: 'text',
                                params: {
                                    spec_id: e.record.data.spec_id_one,
                                    spec_status: e.value
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        VendorProductSpecStore.load();
                                    } else {
                                        VendorProductSpecStore.load();
                                    }

                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    }
                    else {
                        if (e.value != e.originalValue)
                        {
                            Ext.Msg.alert("提示信息", "該商品細項下無規格,不能進行設置!");
                            VendorProductSpecStore.load();
                        }
                    }
                }

                if (e.field == "spec_status_two") {
                    //如果轉移數量不為零的話
                    if (e.originalValue != "" && e.originalValue != null) {
                        if (e.value != e.originalValue) {
                            Ext.Ajax.request({
                                url: "/VendorStock/UpdateStatus",//顯示或者隱藏
                                method: 'post',
                                type: 'text',
                                params: {
                                    spec_id: e.record.data.spec_id_two,
                                    spec_status: e.value
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        VendorProductSpecStore.load();
                                    } else {
                                        VendorProductSpecStore.load();
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    }
                    else {
                        if (e.value != e.originalValue) {
                            Ext.Msg.alert("提示信息", "該商品細項下無規格,不能進行設置!");
                            VendorProductSpecStore.load();
                        }
                    }
                }

            }
        }
    });

    var editSpecFrm = Ext.create('Ext.form.Panel', {
        width: 650,
        height: 450,
        border: false,
        plain: true,
        defaultType: 'displayfield',
        id: 'ImportFile',
        layout: {
            type: 'hbox'
        },
        items: [gdSpecMeg]
    });
    var editWins = Ext.create('Ext.window.Window', {
        title: PRODUCTITEMMESSAGE,
        id: 'editWins',
        width: 650,
        height: 480,
        layout: 'fit',
        items: [editSpecFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: CLOSE,
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ]
    });

    editWins.show();
}