editPaperClassFunction = function (row, store) {

    var old_paper_id = '';
    var PaperListStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        //pageSize: pageSize,
        model: 'gigade.Paper',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: row != null ? '/Paper/GetPaperList?isPage=false' : '/Paper/GetPaperList?status=1&&isPage=false',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
                //totalProperty: 'totalCount'
            }
        }
        //    autoLoad: true
    });
    var class_type_data = [];
    if (row == null) {
        class_type_data = [
                { "txt": '單選', "value": "SC" },
                { "txt": '多選', "value": "MC" },
                { "txt": '單行', "value": "SL" },
                { "txt": '多行', "value": "ML" }
        ];

    }
    else if (row.data.classType == "SC" || row.data.classType == "MC") {
        class_type_data = [
                  { "txt": '單選', "value": "SC" },
                  { "txt": '多選', "value": "MC" }

        ];
    }
    else if (row.data.classType == "SL" || row.data.classType == "ML") {
        class_type_data = [
                 { "txt": '單行', "value": "SL" },
                 { "txt": '多行', "value": "ML" }

        ];
    }
    var ClassTypeStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: class_type_data
    });


    var class_id = '';
    if (row != null) {
        class_id = row.data.classID;
    }
    else {
        class_id = 0;
    }
    Ext.define('gigade.Content', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "id", type: "int" },
            { name: "orderNum", type: "int" },
            { name: "classContent", type: "string" }

        ]
    });

    var ClassContentStore = Ext.create('Ext.data.Store', {
        storeId: 'ClassContentStore',
        model: 'gigade.Content',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/Paper/GetPaperClassList',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    //ClassContentStore.on('beforeload', function () {
    //    Ext.apply(ClassContentStore.proxy.extraParams, {
    //        class_id: Ext.getCmp('classID') == null ? "" : Ext.getCmp('classID').getValue()
    //    });
    //});
    var channelTpl = new Ext.XTemplate(
       '<a href="#">' + '編輯' + '</a> &nbsp; &nbsp;');


    var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 1,
        autoCancel: false,
        clicksToEdit: 1,
        errorSummary: false,
        listeners: {
            beforeedit: function (e, eOpts) {
                if (e.colIdx == 0) {
                    e.hide();
                }

            }
        }
    });
    Ext.grid.RowEditor.prototype.saveBtnText = '保存';
    Ext.grid.RowEditor.prototype.cancelBtnText = '取消';

    function onAdd() {
        var opcontent = Ext.getCmp("opcontent");
        var order = Ext.getCmp("order");
        if (opcontent.getValue() != "" && order.getValue().toString() != "") {
            ClassContentStore.add({
                classContent: opcontent.getValue(),
                orderNum: order.getValue()
            });
            opcontent.setValue('');
            order.setValue(0);
            ClassContentStore.sort('orderNum', 'asc');

        }
        else {
            return;
        }

    }

    var extGrid = new Ext.grid.Panel(
    {
        id: 'gdContent',
        store: ClassContentStore,
        height: 155,
        columnLines: true,
        plugins: [rowEditing],
        frame: true,
        columns: [
        {
            header: '刪除', xtype: 'actioncolumn', width: 40, align: 'center',
            items: [{
                icon: '../../../Content/img/icons/cross.gif',
                tooltip: '刪除',
                handler: function (grid, rowIndex, colIndex) {
                    //提示是否刪除
                    var rec = grid.getStore().getAt(rowIndex);
                    var id = rec.get('id');
                    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, 1), function (btn) {
                        if (btn == 'yes') {
                            //編輯時，刪除一條數據，並且這條數據是剛新增的，尚未保存到數據庫中
                            if (row != null && id != 0) {
                                var count = Ext.getCmp("gdContent").getStore().data.length;
                                //至少保留一條數據
                                if (count > 1) {
                                    Ext.Ajax.request({
                                        url: '/Paper/PaperClassDelete',
                                        method: 'post',
                                        params: { id: id },
                                        success: function (form, action) {
                                            var result = Ext.decode(form.responseText);
                                            if (result.success) {
                                                ClassContentStore.load({ params: { class_id: class_id } });
                                                ClassContentStore.sort('orderNum', 'asc');
                                                PaperClassStore.load();
                                            }
                                        },
                                        failure: function () {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    });
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "數據不能為空！");
                                }
                            }
                            else {
                                ClassContentStore.removeAt(rowIndex);
                                ClassContentStore.sort('orderNum', 'asc');
                            }

                        }
                    });
                }
            }]
        },
{
    header: '選項內容', dataIndex: 'classContent', width: 270, align: 'center',
    editor: {
        xtype: "textfield",
        allowBlank: false
    }
},
{
    header: '選項排序', dataIndex: 'orderNum', width: 56, align: 'center', editor: {
        xtype: "numberfield",
        allowBlank: false,
        minValue: 0,
        value: 0
    }
},
{ header: '編輯', xtype: 'templatecolumn', tpl: channelTpl, width: 70, align: 'center' }
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
    if (class_id != 0) {
        if (row.data.classType == "SC" || row.data.classType == "MC") {
            ClassContentStore.load({ params: { class_id: class_id } });
            ClassContentStore.sort('orderNum', 'asc');
        }
    }

    var editPaperClassFm = Ext.create('Ext.form.Panel', {
        id: 'editPaperClassFm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 80,
        url: '/Paper/PaperClassEdit',
        defaults: { anchor: "97%", msgTarget: "side" },
        items: [
            {
                xtype: 'combobox',
                id: 'paperID',
                name: 'paperID',
                fieldLabel: '問卷名稱',
                queryMode: 'local',
                editable: false,
                store: PaperListStore,
                displayField: 'paperName',
                valueField: 'paperID',
                value: 0,
                allowBlank: false
            },
            {
                xtype: 'displayfield',
                fieldLabel: '題目編號',
                id: 'classID',
                name: 'classID',
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '題目名稱',
                id: 'className',
                name: 'className',
                allowBlank: false,
                submitValue: true
            },
             {
                 xtype: 'combobox',
                 id: 'classType',
                 name: 'classType',
                 fieldLabel: '題目類型',
                 queryMode: 'local',
                 editable: false,
                 store: ClassTypeStore,
                 displayField: 'txt',
                 valueField: 'value',
                 value: 'SC',
                 allowBlank: false,
                 listeners: {
                     change: function (radio, newValue, oldValue) {
                         if (newValue == "SL" || newValue == "ML") {
                             Ext.getCmp("opcontent").hide();
                             Ext.getCmp("order").hide();
                             Ext.getCmp("p_add").hide();
                             Ext.getCmp("gdContent").hide();
                         }
                         else {
                             Ext.getCmp("opcontent").show();
                             Ext.getCmp("order").show();
                             Ext.getCmp("p_add").show();
                             Ext.getCmp("gdContent").show();
                         }
                     }
                 }
             },
            {
                xtype: 'numberfield',
                fieldLabel: '題目順序',
                id: 'projectNum',
                name: 'projectNum',
                submitValue: true,
                minValue: 0,
                value: 0,
                allowBlank: false
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 //defaults: {
                 //    flex: 0.5
                 //},
                 width: 500,
                 items: [
                     {
                         xtype: 'displayfield',
                         value: '是否必填'
                     },
                     {
                         xtype: 'radiogroup',
                         allowBlank: false,
                         columns: 2,
                         width: 400,
                         margin: '0 0 0 25',
                         items: [{
                             boxLabel: '是',
                             name: 'isMust',
                             id: 'ym',
                             inputValue: 1
                         },
                         {
                             boxLabel: '否',
                             name: 'isMust',
                             id: 'nm',
                             checked: true,
                             inputValue: 0
                         }]
                     }

                 ]
             }, {
                 xtype: 'fieldcontainer',
                 defaults: {
                     labelWidth: 50,
                     width: 180,
                     margin: '0 5 0 0'
                 },
                 id: 'ls',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                          {
                              xtype: "textareafield",
                              fieldLabel: '選項內容',
                              id: 'opcontent',
                              name: 'opcontent',
                              minValue: 0
                          },
                          {
                              xtype: "numberfield",
                              fieldLabel: '選項排序',
                              id: 'order',
                              name: 'order',
                              minValue: 0,
                              value: 0
                          }, {
                              xtype: 'button',
                              text: ADD,
                              id: 'p_add',
                              width: 40,
                              handler: onAdd

                          }]
             }, extGrid],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                var optioncontent = "";
                var count = Ext.getCmp("gdContent").getStore().data.length;
                if (form.isValid()) {
                    if (Ext.getCmp('classType').getValue() == "SC" || Ext.getCmp('classType').getValue() == "MC") {
                        if (count > 0) {
                            var gdcontent = Ext.getCmp("gdContent").store.data.items;
                            for (var a = 0; a < gdcontent.length; a++) {
                                var class_content = gdcontent[a].get("classContent");
                                var ordernum = gdcontent[a].get("orderNum");
                                var id = gdcontent[a].get("id");
                                optioncontent += class_content + ',' + ordernum + ',' + id + ';';
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, '選項內容不能為空！');
                            return;
                        }
                    }
                    form.submit({
                        params: {
                            paper_id: Ext.htmlEncode(Ext.getCmp('paperID').getValue()),
                            old_paper_id: old_paper_id,
                            id: row == null ? 0 : row.data.id,//題目類型不是單選和多選時要用到
                            class_id: Ext.htmlEncode(Ext.getCmp('classID').getValue()),
                            class_name: Ext.htmlEncode(Ext.getCmp('className').getValue()),
                            class_type: Ext.htmlEncode(Ext.getCmp('classType').getValue()),
                            project_num: Ext.htmlEncode(Ext.getCmp('projectNum').getValue()),
                            ym: Ext.htmlEncode(Ext.getCmp('ym').getValue()),
                            nm: Ext.htmlEncode(Ext.getCmp('nm').getValue()),
                            pcontent: optioncontent
                        },
                        success: function (form, action) {
                            var result = Ext.JSON.decode(action.response.responseText);
                            if (result.success) {
                                if (row == null) {
                                    Ext.Msg.alert(INFORMATION, "新增成功！");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "修改成功！");
                                }
                                store.load();
                                editPaperClassWin.close();
                                editPaperClassWin.destroy();

                            }

                        },
                        failure: function (form, action) {
                            var result = Ext.JSON.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                            store.load();
                            editPaperClassWin.close();
                            editPaperClassWin.destroy();

                        }
                    });
                }

            }
        }]
    });
    var editPaperClassWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editPaperClassWin',
        width: 500,
        height: 450,
        layout: 'fit',
        items: [editPaperClassFm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        title: row == null ? '問卷題目新增' : '問卷題目編輯',
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
                         editPaperClassWin.destroy();
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
                if (row != null) {
                    Ext.getCmp('classID').show();
                    old_paper_id = row.data.paperID;
                    editPaperClassFm.getForm().loadRecord(row);
                }
            }
        }
    });
    editPaperClassWin.show();
}
