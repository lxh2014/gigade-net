
function editFunction(RowID,Store) {
       var t_zip_codeStoreEdit = Ext.create('Ext.data.Store', {
            model: 'gigade.t_zip_code',
            autoDestroy: true,
            autoLoad: true,
            proxy: {
                type: 'ajax',
                url: '/DeliveryStore/GetTZipCodeList?type=edit',
                actionMethods: 'post',
                reader: {
                    type: 'json',
                    root: 'data'
                }
            }
        });
        var DspDeliverStoreEdit = Ext.create('Ext.data.Store', {
            model: 'gigade.DspDeliverStore',
            autoDestroy: true,
            autoLoad: true,
            proxy: {
                type: 'ajax',
                url: '/DeliveryStore/GetDspDeliverStoreList?type=edit',
                actionMethods: 'post',
                reader: {
                    type: 'json',
                    root: 'data'
                }
            }
        });
    var ID = null;
    if (RowID != null) {
        ID = RowID.data["dsp_id"];
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,//True 为 Panel 填充画面,默认为false.
        plain: true,//不在选项卡栏上显示全部背景。
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/DeliveryStore/DeliveryStorePlaceSave',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: "營業所名稱",
                name: 'dsp_name',
                id: 'dsp_name',
                margin: '10 0',
                allowBlank: false,
                maxLength: 50
            },
            {
                xtype: 'textfield',
                fieldLabel: "營業所地址",
                id: 'dsp_address',
                name: 'dsp_address',
                margin: '10 0',
                allowBlank: false,
                maxLength: 125
            },
            {
                xtype: 'textfield',
                fieldLabel: "營業所電話",
                id: 'dsp_telephone',
                margin: '10 0',
                name: 'dsp_telephone',
                allowBlank: false,
                maxLength: 50
            },
              {
                  xtype: 'combobox',
                  fieldLabel: "區域",
                  margin: '10 0',
                  allowBlank: true,//可以為空
                  editable: false,//阻止直接在表单项的文本框中输入字符
                  id: 'dsp_big_code',
                  name: 'dsp_big_code',
                  store: t_zip_codeStoreEdit,
                  displayField: 'big',
                  valueField: 'bigcode',
                  emptyText: '請選擇',
                  width:160,
                  value:'',
                  queryMode: 'local'
              }, 
               {
                   xtype: 'combobox',
                   fieldLabel: "所屬公司",
                   margin: '10 0',
                   allowBlank: true,
                   editable: false,//阻止直接在表单项的文本框中输入字符
                   id: 'dsp_deliver_store',
                   name: 'dsp_deliver_store',
                   store: DspDeliverStoreEdit,
                   displayField: 'parameterName',
                   valueField: 'ParameterCode',
                   emptyText: '請選擇',
                   width: 160,
                   queryMode:'local',
                   value: ''
               },
                 {
                     xtype: 'textareafield',
                     fieldLabel: "備註",
                     id: 'dsp_note',
                     margin: '10 0',
                     name: 'dsp_note',
                     allowBlank: true,
                     height: 100,
                     maxLength:250
                 }
        ],
        buttons: [
        {
            text: '保存',
            formBind: true,// 设置按钮与表单绑定，需在表单验证通过后才可使用
            handler: function () {
                var form = this.up('form').getForm();//沿着 ownerCt 查找匹配简单选择器的祖先容器.
                if(Ext.getCmp("dsp_big_code").getValue()=="")
                {
                    Ext.Msg.alert(INFORMATION, "請選擇區域!");
                    return false;
                }
                if(Ext.getCmp("dsp_deliver_store").getValue()=="")
                {
                    Ext.Msg.alert(INFORMATION, "請選擇公司!");
                    return false;
                }
                if (form.isValid()) {//这个函数会调用已经定义的校验规则来验证输入框中的值，如果通过则返回true
                    form.submit({
                        params: {
                            dsp_id: ID,
                            dsp_name: Ext.htmlEncode(Ext.getCmp("dsp_name").getValue()),
                            dsp_address: Ext.htmlEncode(Ext.getCmp("dsp_address").getValue()),
                            dsp_telephone: Ext.htmlEncode(Ext.getCmp("dsp_telephone").getValue()),
                            dsp_big_code: Ext.htmlEncode(Ext.getCmp("dsp_big_code").getValue()),
                            dsp_deliver_store: Ext.htmlEncode(Ext.getCmp("dsp_deliver_store").getValue()),
                            dsp_note: Ext.htmlEncode(Ext.getCmp("dsp_note").getValue()),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success == "true") {
                                Ext.Msg.alert(INFORMATION, "保存成功!");
                                editWin.close();
                                Store.removeAll();
                                Store.load();
                            } else {
                                if (result.success == "-1") {
                                    Ext.Msg.alert(INFORMATION, '已存在該營業所名稱');
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }
                        }
                    })
                }
            }
        }
      ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: RowID ? '編輯營業所':'增加營業所',
        id: 'editWin',
        iconCls: RowID ? 'icon-user-edit': 'icon-user-add',
        width: 500,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        //closeAction: 'hide',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '關閉',
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
                if (RowID) {
                    editFrm.getForm().loadRecord(RowID); //如果是編輯的話,加載選中行數據,载入一个 Ext.data.Model 到表单中
                }
                else {
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
}