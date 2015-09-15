function showChildFun(olderChildStore, fatherName) {
    var orderChildGrid = new Ext.grid.Panel({
        id: 'orderChildGrid',
        store: olderChildStore,
        region: 'center',
        autoScroll: true,
        border: 0,
        height: 300,
        columns: [
                  {
                      header: '出貨商', dataIndex: 'Vendor_Name_Simple', width: 110, align: 'center'
                      //,
                      //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      //    return '<a href="#"  onclick="">' + value + '</a>&nbsp;&nbsp;'
                      //    //<a href="#"  onclick="">出貨</a>
                      //}
                  },
                  { header: '訂單編號', dataIndex: 'Slave_Id', width: 55, align: 'center' },
                  { header: '訂單狀態', dataIndex: 'Slave_Status_Str', width: 55, align: 'center' },
                  {
                      header: '訂單歸檔日', dataIndex: 'Clos_Date', width: 120, align: 'center',
                      renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                          if (value == '1970-01-01 08:00:00') {
                              return "";
                          }
                          else {
                              return value;
                          }

                      }
                  },
                  {
                      header: '購物編號', dataIndex: 'Detail_Id', width: 80, align: 'center',
                      renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                          return '<a href="#"  onclick="showChildDetail(' + value + ')">' + value + '</a>'
                          //顯示購物編號詳情
                      }
                  },
                  { header: '商品狀態', dataIndex: 'Detail_Status_Str', width: 80, align: 'center' },
                  {
                      header: '商品編號', colName: 'Item_Id', width: 60, align: 'center',
                      renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                          return '<a href="#"  onclick="javascript:showProductDetail(' + record.data.Product_Id + ')">' + record.data.Item_Id + '</a>'

                      }
                  },
                  {
                      header: '商品名稱', dataIndex: 'Product_Name', width: 200, align: 'left',
                      renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                          var value_str = record.data.Brand_Name + "---" + value + record.data.Product_Spec_Name;

                          if (record.data.Detail_Note != null && record.data.Detail_Note != "") {
                              value_str += '<br /><span style="color:#f0f;">&nbsp;&nbsp;' + record.data.Detail_Note + '</span>';
                          }

                          return value_str;

                      }
                  },
                  { header: '托運單屬性', dataIndex: 'Product_Freight_Set_Str', width: 80, align: 'center' },
                  {
                      header: '營業稅', dataIndex: 'Tax_Type', width: 55, align: 'center',
                      renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                          switch (value) {
                              case "1": return "應稅"; break;
                              case "3": return "免稅"; break;
                          }
                      }
                  },
                  { header: '數量', dataIndex: 'Buy_Num', width: 50, align: 'center' }

        ],

        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    var frm = new Ext.form.Panel({
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        items: [
            orderChildGrid
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "組合商品詳情【" + fatherName + "】",
        width: 1000,
        y: 100,
        layout: 'fit',
        constrain: true,
        items: [frm],
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: CLOSEFORM,
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            editWin.destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {

                frm.getForm().reset();

            }
        }
    });
    editWin.show();

    showChildDetail = function (DetailId) {
        var record = olderChildStore.getAt(olderChildStore.find("Detail_Id", DetailId));
        if (record != undefined && record != null) {
            showDetailFun(record);
        }
    }

}
