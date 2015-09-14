TranToDetailBySMD = function (BatchDetailStore) {

    //顯示類別中商品的grid
    var Grid = new Ext.grid.Panel({
        id: 'Grid',
        store: BatchDetailStore,
        autoScroll: true,
        region: 'center',
        height: 320,
        columnLines: true,
        columns: [
              {
                  header: '付款單號', dataIndex: 'order_id', width: 100, align: 'center',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      if (value == 0) {
                          return "--";
                      }
                      else {
                          return value;
                      }
                  }
              },
                {
                    header: '廠商出貨單號', dataIndex: 'slave_id', width: 100, align: 'center',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                        if (value == 0) {
                            return "--";
                        }
                        else {
                            return value;
                        }
                    }
                },
              { header: '商品編號', dataIndex: 'item_id', width: 100, align: 'center' },
              {
                  header: '商品名稱', dataIndex: 'product_name', width: 280, align: 'center',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      return value + record.data.product_spec_name;
                  }
              },
              { header: '數量', dataIndex: 'buy_num', width: 60, align: 'center' }
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

    var detailFrm = Ext.create('Ext.form.Panel', {
        id: 'detailFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        items: [
                   Grid
        ]
    });
    var detailWin = Ext.create('Ext.window.Window', {
        title: "廠商出貨明細",
        iconCls: 'icon-user-edit',
        id: 'detailWin',
        width: 600,
        y: 100,
        layout: 'fit',
        items: [detailFrm],
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
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('detailWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ]

    });


    detailWin.show();




}