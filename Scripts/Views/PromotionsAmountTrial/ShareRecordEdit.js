Ext.define('TrialPicture', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "share_id", type: "int" },
    { name: "image_filename", type: "string" },
    { name: "image_sort", type: "uint" },
    { name: "image_state", type: "uint" },
    { name: "image_createdate", type: "uint" },
    ]

});
var TrialPictureStore;
var isChecked = 0;
editFunction = function (rowID) {
    var row = null;
    if (rowID != null) {
        edit_ShareRecordStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_ShareRecordStore.getAt(0);
                editWin.show();
            }
        });
    }
    else {
        editWin.show();
    }
    TrialPictureStore = Ext.create('Ext.data.Store', {
        model: 'TrialPicture',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/PromotionsAmountTrial/QueryPic',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        },
        sorters: [{
            property: 'image_sort',
            direction: 'DESC'
        }]
    });

    TrialPictureStore.on('beforeload', function () {
        Ext.apply(TrialPictureStore.proxy.extraParams,
    {
        share_id: Ext.getCmp("share_id").getValue()
    });
    });
    var cellEditingEx = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });
    var pic = Ext.create('Ext.grid.Panel', {
        id: 'picPanel',
        plugins: [cellEditingEx],
        height: 150,
        store: TrialPictureStore,
        columns: [
                 {
                     xtype: 'actioncolumn',
                     width: 100,
                     id: 'deleteEx',
                     colName: 'deleteEx',
                     align: 'center',
                     items: [
                         {
                             icon: '../../../Content/img/icons/cross.gif',
                             tooltip: DELETE,
                             handler: function (grid, rowIndex, colIndex) {

                                 Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, 1), function (btn) {
                                     if (btn == "yes") {
                                         TrialPictureStore.removeAt(rowIndex);
                                     }
                                 });
                             }
                         }
                     ]
                 },
                 {
                     header: SHAREIMAGE, dataIndex: 'image_filename', width: 60, align: 'center',
                     xtype: 'templatecolumn',
                     tpl: '<img width=50 name="tplImg"  height=50 src="{image_filename}" />'
                 },
                 {
                     header: STATUS, dataIndex: 'image_state', width: 60, align: 'center', sortable: false,
                     menuDisabled: true,
                     renderer: function (value) {
                         if (value == 1 || value == "true") {
                             return SHOWSTATUS;
                         }
                         else {
                             return HIDESTATUS;
                         }
                     },
                     editor: {
                         xtype: 'checkboxfield',
                         width: 40,
                         labelWidth: 30
                     }
                 },
             { header: SORT, dataIndex: 'image_sort', width: 60, align: 'center', editor: { xtype: 'numberfield', minValue: 0 } }
        ],
        plugins: [
        Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
        })
        ]
        ,
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
 
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'form',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        //  defaults: { anchor: "95%", msgTarget: "side" },
        url: '/PromotionsAmountTrial/TrialRecordSave',
        items:
            [
            {
                xtype: 'displayfield',
                fieldLabel: "編號",
                id: 'share_id',
                name: 'share_id',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: "活動名稱",
                id: 'event_name',
                name: 'event_name',
                width: 550,
            },
            {
                xtype: 'displayfield',
                fieldLabel: HDID,//活動編號
                id: 'trial_id',
                name: 'trial_id',
                value: document.getElementById("trial_id").value,
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: "用戶編號",
                id: 'user_id',
                name: 'user_id',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: "註冊所用用戶名",
                id: 'real_name',
                name: 'real_name',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: "更改之後所用用戶名",
                id: 'after_name',
                name: 'after_name',
                hidden: true
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'textfield',
                         fieldLabel: NICKNAME,
                         allowBlank: false,
                         id: 'user_name',
                         name: 'user_name',
                         listeners: {
                             beforerender: function () {
                                 var checkValue = (Ext.getCmp('niming').getValue());
                                 if (!checkValue) {
                                     Ext.getCmp('user_name').setReadOnly(true);
                                 }
                             }
                         },
                     },
        {
            xtype: 'checkbox',
            boxLabel: NONAME,
            margin: '0 0 0 20',
            id: 'niming',
            name: 'niming',
            handler: function () {
                var checkValue = (Ext.getCmp('niming').getValue());
                var real_name = (Ext.getCmp('real_name').getValue());
                var user_name = Ext.getCmp('user_name').getValue();
                var after_name = Ext.getCmp('after_name').getValue();
                if (checkValue) {
                    if (user_name == real_name) {
                        if (Ext.getCmp('gender').getValue() == MISS) {
                            Ext.getCmp('user_name').setReadOnly(false);
                            Ext.getCmp('user_name').setValue(MISS);
                            Ext.getCmp('sex1').setValue(true);
                        }
                        else {
                            Ext.getCmp('user_name').setReadOnly(false);
                            Ext.getCmp('user_name').setValue(SIR);
                            Ext.getCmp('sex2').setValue(true);
                        }
                    }
                    else {

                        Ext.getCmp('user_name').setValue(after_name);
                        Ext.getCmp('user_name').setReadOnly(false);
                    }
                }
                else {
                    Ext.getCmp('user_name').setValue(Ext.getCmp('real_name').getValue());
                    Ext.getCmp('user_name').setReadOnly(true);
                }
                // }
            }

        }
                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'hidden_性別',//0：小姐 ；1： 先生。顯示數字
                id: 'gender',
                name: 'gender',
                hidden: true,
            },
            {
                xtype: 'radiogroup',
                fieldLabel: '性別',
                id: 'user_gender',
                name: 'user_gender',
                width: 270,
                defaults: {
                    flex: 1,
                    name: 'user_gender',
                },
                items: [
                {
                    id: 'sex1', inputValue: '0', boxLabel: '小姐', checked: true
                },
                {
                    id: 'sex2', inputValue: '1', boxLabel: '先生'
                }
                ],
                listeners: {
                    change: function () {
                        var user_gender = Ext.getCmp('user_gender').getValue().user_gender;
                        var checkValue = (Ext.getCmp('niming').getValue());
                        if (user_gender == 0 && checkValue) {
                            Ext.getCmp('user_name').setValue(MISS);
                        }
                        else if (user_gender == 1 && checkValue) {
                            Ext.getCmp('user_name').setValue(SIR);
                        }
                    }
                }
            },
            {
                xtype: 'textarea',
                fieldLabel: SHARECONTENT,
                //maxLength: 50,
                //maxLengthText: '最多分享50字',
                width: 350,
                id: 'content',
                name: 'content'
            },
            {
                xtype: 'radiogroup',
                fieldLabel: '狀態',
                id: 'status',
                name: 'status',
                defaultType: 'radiofield',
                defaults: {
                    flex: 1,
                    name: 'status',
                },
                columns: 3,
                vertical: true,
                items: [
                                        {
                                            boxLabel: '新建立',
                                            id: 'new',
                                            inputValue: 0,
                                            checked: true
                                        },
                      {
                          boxLabel: '顯示',
                          id: 'show',
                          inputValue: 1
                      },
                        {
                            boxLabel: '隱藏',
                            id: 'hide',
                            inputValue: 2
                        },
                          {
                              boxLabel: '下檔',
                              id: 'down',
                              inputValue: 3
                          }
                ]
            },
            {
                xtype: 'displayfield',
                id: 'share_time',
                name: 'share_time',
                fieldLabel: '分享時間',
                width: 300,
            },
            {
                xtype: 'button',
                text: ADDIMAGE,
                iconCls: 'icon-add',
                handler: function () {
                    addPic();
                }
            },
           pic
            ],
        buttons: [
           {
               text: SAVE,
               id: 'btn',
               formBind: true,
               disabled: true,
               handler: function () {
                   var form = this.up('form').getForm();
                   if (form.isValid()) {
                       form.submit({
                           params: {
                               picInfo: savePic(),
                               share_id: Ext.htmlEncode(Ext.getCmp('share_id').getValue()),
                               trial_id: Ext.htmlEncode(Ext.getCmp('trial_id').getValue()),
                               user_name: Ext.htmlEncode(Ext.getCmp('user_name').getValue()),
                               user_gender: Ext.htmlEncode(Ext.getCmp('user_gender').getValue().user_gender),
                               niming: (Ext.getCmp('niming').getValue()),
                               status: Ext.getCmp('status').getValue().status,
                               content: Ext.htmlEncode(Ext.getCmp('content').getValue())
                           },
                           success: function (form, action) {
                               var result = Ext.decode(action.response.responseText);
                               if (result.success) {
                                   Ext.Msg.alert(INFORMATION, SUCCESS);
                                   ShareRecordStore.load();
                                   editWin.close();
                               }
                               else {
                                   Ext.Msg.alert(INFORMATION, FAILURE);
                                   ShareRecordStore.load();
                                   editWin.close();
                               }
                           },
                           failure: function (form, action) {
                               Ext.Msg.alert(INFORMATION, FAILURE);
                           }
                       });
                   }
               }

           }
        ]
    })

    var editWin = Ext.create('Ext.window.Window', {
        title: SHARERECORD,
        id: 'editWin',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        width: 583,
        height: 571,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
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
                       TrialPictureStore.destroy();
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
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話
                }
                else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                    TrialPictureStore.load();
                    initForm(row);
                }
            }
        }
    });
    //editWin.show();
    function initForm(Row) {
        switch (Row.data.status) {
            case 0:
                Ext.getCmp('new').setValue(true);
                break;
            case 1:
                Ext.getCmp('show').setValue(true);
                break;
            case 2:
                Ext.getCmp('hide').setValue(true);
                break;
            case 3:
                Ext.getCmp('down').setValue(true);
                break;
        }
        if (row.data.gender == MISS) {
            Ext.getCmp('sex1').setValue(true);
        }
        else if (row.data.gender == SIR) {
            Ext.getCmp('sex2').setValue(true);
        }
        if (Row.data.is_show_name == 0) {
            isChecked = 1;
            Ext.getCmp('niming').setValue(true);
        }
    }
    //保存圖片
    function savePic() {
        var pictureInfo = Ext.getCmp('picPanel').store.data.items;
        // alert(pictureInfo[0].get("image_sort"));
        var insertPictureInfo = "";
        for (var i = 0; i < pictureInfo.length; i++) {
            var record_id = pictureInfo[i].get("record_id");
            var image_filename = pictureInfo[i].get("image_filename");
            var image_state = pictureInfo[i].get("image_state");
            var image_sort = pictureInfo[i].get("image_sort");
            if (image_state == true) {
                image_state = 1;
            }
            else {
                image_state = 0;
            }
            insertPictureInfo += record_id + "," + image_filename.substring(image_filename.lastIndexOf("/") + 1) + "," + image_sort + "," + image_state + ";";
        }
        return insertPictureInfo;
    }
    //上傳圖片
    function addPic() {
        var picGrid = Ext.getCmp("picPanel").store.data.items;
        if (picGrid.length >= 5) {
            Ext.Msg.alert(INFORMATION, MAXIMAGES);
            return;
        }
        else {
            $(window.frames[0].document).find("#Filedata").click();
        }

    }
}
function addNewRow(imgPath, type) {
    var r = Ext.create('TrialPicture', {

        record_id: 0,
        image_filename: imgPath,
        image_sort: 0,
        image_state: 1,
        image_createdate: 0
    });
    TrialPictureStore.insert(0, r);
}
