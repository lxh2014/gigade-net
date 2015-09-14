function showDetailFun(row) {

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: "數量",
                editable: false,
                colName: 'Buy_Num',
                id: 'Buy_Num',
                name: 'Buy_Num',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "售價成本",
                editable: false,
                colName: 'Single_Cost',
                id: 'Single_Cost',
                name: 'Single_Cost',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "活動成本",
                editable: false,
                colName: 'Event_Cost',
                id: 'Event_Cost',
                name: 'Event_Cost',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "售價",
                editable: false,
                colName: 'Single_Price',
                id: 'Single_Price',
                name: 'Single_Price',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "購買價",
                editable: false,
                colName: 'Single_Money',
                id: 'Single_Money',
                name: 'Single_Money',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "父商品數量",
                editable: false,
                colName: 'parent_num',
                id: 'parent_num',
                name: 'parent_num',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "寄倉費",
                editable: false,
                colName: 'Bag_Check_Money',
                id: 'Bag_Check_Money',
                name: 'Bag_Check_Money',
                style: { borderBottom: '1px solid #ced9e7', paddingTop: '5px' }
            }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "購物詳情",
        id: 'editWin',
        width: 500,
        height: 310,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                   
                }
                else {
                    editFrm.getForm().reset();
                }

            }
        }
    });
    editWin.show();
   
}
