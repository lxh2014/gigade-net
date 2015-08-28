editBannerImageHistoryFunction = function (row, store) {
    var editBannerImageHostoryFrm = Ext.create('Ext.form.Panel', {
        id: 'editBannerImageHostoryFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 80,
        url: '/Website/BannerImageEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '圖片位置',
                id: 'site_name',
                name: 'site_name',
                value: document.getElementById('sname').value
            },
            {
                xtype: 'displayfield',
                fieldLabel: '流水號',
                id: 'banner_content_id',
                name: 'banner_content_id'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '文字',
                id: 'banner_title',
                name: 'banner_title'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '連結',
                id: 'banner_link_url',
                name: 'banner_link_url'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '開啟模式',
                id: 'banner_link_mode',
                name: 'banner_link_mode'
            },
             //{
             //    xtype: 'fieldcontainer',
             //    combineErrors: true,
             //    layout: 'hbox',
             //    //defaults: {
             //    //    flex: 0.5
             //    //},
             //    width: 500,
             //    items: [
             //        {
             //            xtype: 'displayfield',
             //            value: '開啟模式',
             //            labelWidth: 100
             //        },
             //        {
             //            xtype: 'radiogroup',
             //            allowBlank: false,
             //            width: 200,
             //            columns: 2,
             //            margin: '0 0 0 10',
             //            items: [{
             //                boxLabel: '母視窗連接',
             //                name: 'banner_link_mode',
             //                id: 'oldw',
             //                checked: true,
             //                inputValue: 1
             //            },
             //            {
             //                boxLabel: '新視窗開啟',
             //                name: 'banner_link_mode',
             //                id: 'neww',
             //                inputValue: 2
             //            }]
             //        }

             //    ]
             //},
            {
                xtype: 'displayfield',
                fieldLabel: '排序',
                id: 'banner_sort',
                name: 'banner_sort',
                submitValue: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '上線時間',
                id: 'banner_start',
                name: 'banner_start'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '下線時間',
                id: 'banner_end',
                name: 'banner_end'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '狀態',
                id: 'banner_status',
                name: 'banner_status'
            },
            {
                 xtype: 'box', //或者xtype: 'component',  
                 width: 100, //图片宽度  
                 height: 100, //图片高度  
                 id: 'banner_image',
                 name: 'banner_image',
                 autoEl: {
                     tag: 'img',    //指定为img标签  
                     src: 'myphoto.gif'    //指定url路径  
                 }
             },
            {
                xtype: 'displayfield',
                fieldLabel: '建立時間',
                id: 'banner_createdate',
                name: 'banner_createdate'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '修改時間',
                id: 'banner_updatedate',
                name: 'banner_updatedate'
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '來源IP',
                 id: 'banner_ipfrom',
                 name: 'banner_ipfrom'
             }]
    });
    var editBannerImageHistoryWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editBannerImageHistoryWin',
        width: 600,
        layout: 'fit',
        items: [editBannerImageHostoryFrm],
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
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editBannerImageHistoryWin').destroy();
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
                editBannerImageHostoryFrm.getForm().loadRecord(row);
                if (row.data.banner_link_mode == "1") {
                    Ext.getCmp('banner_link_mode').setValue('母視窗鏈接');
                }
                else if (row.data.banner_link_mode == "2") {
                    Ext.getCmp('banner_link_mode').setValue('新視窗鏈接');
                }
                Ext.getCmp('banner_start').setValue(Ext.Date.format(new Date(row.data.banner_start), 'Y-m-d'));
                Ext.getCmp('banner_end').setValue(Ext.Date.format(new Date(row.data.banner_end), 'Y-m-d'));
                if (row.data.banner_status == "0") {
                    Ext.getCmp('banner_status').setValue('新建');

                }
                else if (row.data.banner_status == "1") {
                    Ext.getCmp('banner_status').setValue('顯示');
                }
                else if (row.data.banner_status == "2") {
                    Ext.getCmp('banner_status').setValue('隱藏');
                }
                else if (row.data.banner_status == "3") {
                    Ext.getCmp('banner_status').setValue('下線/刪除');
                }
                Ext.getCmp('banner_image').getEl().dom.src =row.data.banner_image;
            }
        }
    });
    editBannerImageHistoryWin.show();
    //initForm(row);
}
function initForm(row) {
    var img = row.data.banner_image.toString();
    //var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
    var url = img.split("\/");
    var imgUrl = url[url.length - 1];
    Ext.getCmp('banner_image').setRawValue(imgUrl);
}
