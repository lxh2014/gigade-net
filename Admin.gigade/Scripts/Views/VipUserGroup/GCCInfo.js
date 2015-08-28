GCCInfoFunction = function (row) {
    //聯繫人
    Ext.define('gigade.GroupCommitteContact', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "gcc_id", type: "int" },
            { name: "group_id", type: "int" },
            { name: "gcc_chairman", type: "string" },
            { name: "gcc_phone", type: "string" },
            { name: "gcc_mail", type: "string" }
        ]
    });
    var Store = Ext.create('Ext.data.Store', {
        model: 'gigade.GroupCommitteContact',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetGroupCommitteContact",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    var editFrm = Ext.create('Ext.grid.Panel', {
        id: 'gccInfo',
        store: Store,
        anchor: '98%',
        height: 220,
        columnLines: true,
        autoScroll: true,
        frame: true,
        columns: [            
            { header: '福委姓名', dataIndex: 'gcc_chairman',flex:2, align: 'center' },
            { header: '福委電話', dataIndex: 'gcc_phone', flex: 2, align: 'center' },
            { header: '福委mail', dataIndex: 'gcc_mail', flex: 2, align: 'center' }
        ]
    });


    var editWintwo = Ext.create('Ext.window.Window', {
        title: '福委聯絡人',
        iconCls: 'icon-user-edit',
        id: 'editWintwo',
        width: 700,
        layout: 'fit',
        height: 300,
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWintwo').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                Store.load({ params: { group_id: row } });
            }
        }
    });
    editWintwo.show();
}