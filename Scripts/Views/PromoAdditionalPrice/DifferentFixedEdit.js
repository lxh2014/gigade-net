
function editFunction(row, store) {

    SiteStore.load();
    VipGroupStore.load();
    var ID = "";
    var even_name = "";
    var even_desc = "";
    var conditionID = "";
    var event_id = "";
    var websiteID = 1;
    if (row != null) {
        ID = row.data["id"];
        conditionID = row.data["condition_id"]; 
        websiteID = row.data["website"] == "" ? 1 : row.data["website"];
    }
    var currentPanel = 0;
    var promoID = "";
    var produCateID = "";
    var produCateID_one = "";
    var produCateID_two = "";
    var linkPath = "http://www.gigade100.com/promotion/combo_promotion.php?event_id="; //保存图片链接的地址
    var event_type = document.getElementById("event_type").value;
    var navigate = function (panel, direction) {
        // 此程序可以包含一些控制导航步骤的必要业务逻辑. 比如调用setActiveItem, 管理导航按钮的状态,
        // 处理可能出现的分支逻辑, 处理特殊操作像取消或结束等等. 一个完整的向导页, 对于复杂的需求
        // 实现起来可能也会相当复杂, 在实际的程序中通常应该以继承CardLayout的方式来实现. 
        var layout = panel.getLayout();
        var move = Ext.getCmp('move-next').text;

        if ('next' == direction) {
            if (currentPanel == 0) {
                var name = Ext.getCmp("name");
                var desc = Ext.getCmp("desc");
                if (name.isValid() && desc.isValid()) {
                    Ext.Ajax.request({
                        url: '/PromoAdditionalPrice/SaveOne',
                        method: 'post',
                        params: { name: name.getValue(), desc: desc.getValue(), event: event_type },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                promoID = result.id;
                                produCateID = result.cateID;

                                produCateID_one = result.cateOne;

                                produCateID_two = result.cateTwo;
                                event_id = result.event_id;
                                layout[direction]();
                                currentPanel++;
                                if (!layout.getNext()) {
                                    Ext.getCmp('move-next').hide();
                                    Ext.getCmp('move-prev').setDisabled(false);
                                    Ext.getCmp('move-prev').show();
                                }
                                else {
                                    Ext.getCmp('move-next').setText(NEXT_MOVE);
                                }
                            } else {
                                Ext.Msg.alert(INFORMATION, "操作失敗1");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "操作失敗2");
                        }
                    });

                }
            } else {
                if (currentPanel == 1) {
                    var condition = Ext.getCmp("condition_id");
                    var group = Ext.getCmp("group_id");
                    var starts = Ext.getCmp("starts");
                    var end = Ext.getCmp("end");
                }

            }
        } else {
            layout[direction]();
            currentPanel--;
        }
        Ext.getCmp('move-next').show();
        Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
    };
    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                fieldLabel: '活動名稱',
                xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'name',
                name: 'name',
                allowBlank: false,
                labelWidth: 80
            },
            {
                fieldLabel: '活動描述',
                xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'desc',
                name: 'desc',
                allowBlank: false,
                labelWidth: 80
            }
        ]
    });
    //第二步1
    var secondForm = Ext.widget('form', {
        id: 'editFrm',
        frame: true,

        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        // url: '/PromoAdditionalPrice/SaveTwo',
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                //if (Ext.getCmp("usr2").getValue()) {
                //    var text = Ext.getCmp('userInfo1').getValue();
                //    if (text == "" || text == null) {
                //        alert("請選擇會員條件");
                //        return false;
                //    }

                //}
                //if (condition_id == "") {
                //if (condition_id == "" && (Ext.getCmp("groupname1").getValue() == null || Ext.getCmp("groupname1").getValue() == "")) {
                //    Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                //    return;
                //}
                if (form.isValid()) {
                    var regex = /^([0-9]+,)*[0-9]+$/;
                    if (!regex.test(websiteID)) {
                        websiteID = SiteStore.getAt(0).get('Site_id')
                    }
                    Ext.Ajax.request({
                        url: '/PromoAdditionalPrice/DeletLessThen',
                        method: 'post',
                        params: { rowid: promoID, fixed_price: Ext.htmlEncode(Ext.getCmp('fixed_price1').getValue()), producateid: produCateID_two, websiteid: websiteID, types: 2 },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                if (result.delcount == 1) {
                                    Ext.Msg.alert("提示", "低於商品加構價的將會刪除!");
                                    setTimeout(2000);
                                }
                                chaojie = 1;
                                // form.submit({
                                Ext.Ajax.request({
                                    url: '/PromoAdditionalPrice/SaveTwo',
                                    method: 'post',
                                    params: {
                                        rowid: promoID,
                                        categoryid: produCateID,
                                        banner_url: Ext.htmlEncode(Ext.getCmp('banner_link_url1').getValue()),
                                        banner: Ext.htmlEncode(Ext.getCmp('banner_image1').getValue()),
                                        //group_id: Ext.htmlEncode(Ext.getCmp('groupname1').getValue()),
                                        group_id: 0,
                                        //condition_id: condition_id,
                                        condition_id: 0,
                                        deliver_id: Ext.htmlEncode(Ext.getCmp("rd_deliver").getValue().rd_Deliver),
                                        device_id: Ext.htmlEncode(Ext.getCmp("rd_device").getValue().rd_Device),
                                        start_date: Ext.Date.format(new Date(Ext.getCmp('start_date').getValue()), 'Y-m-d H:i:s'),
                                        end_date: Ext.Date.format(new Date(Ext.getCmp('end_date').getValue()), 'Y-m-d H:i:s'),//Ext.htmlEncode(Ext.getCmp('end').getValue()),
                                        //start_date: Ext.htmlEncode(Ext.getCmp('start_date').getValue()),
                                        //end_date: Ext.htmlEncode(Ext.getCmp('end_date').getValue()),
                                        side: Ext.htmlEncode(Ext.getCmp('website1').getValue()),
                                        fixed_price: Ext.htmlEncode(Ext.getCmp('fixed_price1').getValue()),
                                        buy_limit: Ext.htmlEncode(Ext.getCmp('buy_limit1').getValue()),
                                        event_type: document.getElementById("event_type").value,
                                        event_desc: Ext.getCmp("desc").getValue()
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        //var result = Ext.decode(action.response.responseText);
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        if (result.success) {
                                            FaresStore.load();
                                            editWin.close();
                                        }
                                        else {
                                            if (result.msg.toString() == "1") {
                                                Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                            }
                                            else if (result.msg.toString() == "2") {
                                                Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                            }
                                            else if (result.msg.toString() == "3") {
                                                Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
                                            }
                                            else {
                                                Ext.Msg.alert(INFORMATION, FAILURE);
                                            }
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        if (result.msg.toString() == "1") {
                                            Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                        }
                                        else if (result.msg.toString() == "2") {
                                            Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                        }
                                        else if (result.msg.toString() == "3") {
                                            Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    }
                                });
                            } else {
                                Ext.Msg.alert(INFORMATION, "操作失敗1");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "操作失敗2");
                        }
                    });
                    if (chaojie == 1) {

                    }
                }
            }
        }],

        //            form.submit({
        //                params: {
        //                    rowid: promoID,
        //                    categoryid: produCateID,
        //                    banner_url: Ext.htmlEncode(Ext.getCmp('banner_link_url1').getValue()),
        //                    banner: Ext.htmlEncode(Ext.getCmp('banner_image1').getValue()),
        //                    group_id: Ext.htmlEncode(Ext.getCmp('groupname1').getValue()),
        //                    condition_id: condition_id,
        //                    deliver_id: Ext.htmlEncode(Ext.getCmp("rd_deliver").getValue().rd_Deliver),
        //                    device_id: Ext.htmlEncode(Ext.getCmp("rd_device").getValue().rd_Device),
        //                    start_date: Ext.htmlEncode(Ext.getCmp('start_date').getValue()),
        //                    end_date: Ext.htmlEncode(Ext.getCmp('end_date').getValue()),
        //                    side: Ext.htmlEncode(Ext.getCmp('website1').getValue()),
        //                    fixed_price: Ext.htmlEncode(Ext.getCmp('fixed_price1').getValue()),
        //                    buy_limit: Ext.htmlEncode(Ext.getCmp('buy_limit1').getValue()),
        //                    event_type: document.getElementById("event_type").value,
        //                    event_desc: Ext.getCmp("desc").getValue()
        //                },
        //                success: function (form, action) {
        //                    var result = Ext.decode(action.response.responseText);
        //                    Ext.Msg.alert(INFORMATION, SUCCESS);
        //                    if (result.success) {
        //                        FaresStore.load();
        //                        editWin.close();
        //                    }
        //                    else {
        //                        Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
        //                    }
        //                },
        //                failure: function (form, action) {
        //                    var result = Ext.decode(action.response.responseText);
        //                    if (result.msg.toString() == "1") {
        //                        Ext.Msg.alert(INFORMATION, PHOTOSIZE);
        //                    }
        //                    else if (result.msg.toString() == "2") {
        //                        Ext.Msg.alert(INFORMATION, PHOTOTYPE);
        //                    }
        //                    else if (result.msg.toString() == "3") {
        //                        Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
        //                    }
        //                    else {
        //                        Ext.Msg.alert(INFORMATION, FAILURE);
        //                    }
        //                }
        //            });
        //        }
        //    }
        //}],
        //url: '/PromotionsBonus/Save',
        defaults: { anchor: "90%", msgTarget: "side" },
        items: [
            {
                xtype: 'radiogroup',
                fieldLabel: ISBANNERURL,
                id: 'url_by1',
                colName: 'url_by',
                name: 'url_by1',
                defaults: {
                    name: 'url_by'
                },
                columns: 2,
                vertical: true,
                items: [
                {
                    id: 'bt1',
                    boxLabel: YES,
                    inputValue: '1',
                    name: 'bt',
                    width: 100,
                    listeners: {
                        change: function (radio, newValue, oldValue) {
                            var banner_url = Ext.getCmp("banner_link_url1");
                            var banner_image = Ext.getCmp("banner_image1");
                            if (newValue) {
                                banner_url.allowBlank = false;
                                banner_url.show();
                                banner_image.allowBlank = false;
                                banner_image.show();
                                banner_url.setValue(linkPath + event_id);
                            }
                        }
                    }
                },
                {
                    id: 'bt2',
                    boxLabel: NO,
                    checked: true,
                    inputValue: '0',
                    name: 'bt',
                    listeners: {
                        change: function (radio, newValue, oldValue) {
                            var banner_url = Ext.getCmp("banner_link_url1");
                            var banner_image = Ext.getCmp("banner_image1");
                            if (newValue) {
                                //banner_url.allowBlank = true;
                                //banner_url.hide();
                                //banner_url.setValue("");
                                //banner_image.allowBlank = true;
                                //banner_image.hide();
                                //banner_image.setValue("");
                                //banner_url.isValid();
                                //banner_image.isValid();
                                banner_url.setValue("0");
                                banner_url.allowBlank = true;
                                banner_url.setValue("");
                                banner_url.isValid();
                                banner_url.hide();
                                banner_image.setValue("0");
                                banner_image.allowBlank = true;
                                banner_image.setValue("");
                                banner_image.isValid();
                                banner_image.hide();

                            }
                        }
                    }
                }]
            },
            {
                xtype: 'textfield',
                fieldLabel: SELFURL,
                name: 'banner_link_url1',
                id: 'banner_link_url1',
                labelWidth: 80,
                submitValue: true,
                vtype: 'url',
                allowBlank: true,
                hidden: true
            },
            {
                //專區Banner
                xtype: 'filefield',
                name: 'banner_image1',
                id: 'banner_image1',
                fieldLabel: BANNERIMG,
                labelWidth: 60,
                msgTarget: 'side',
                anchor: '90%',
                buttonText: '選擇...',
                allowBlank: true,
                hidden: true,
                fileUpload: true
            },
            //{
            //    //會員群組和會員條件二擇一
            //    xtype: 'fieldcontainer',
            //    fieldLabel: '會員群組',
            //    combineErrors: true,
            //    margins: '0 200 0 0',
            //    layout: 'hbox',
            //    defaults: {
            //        hideLabel: true,
            //        width: 100
            //    },
            //    items: [
            //        {
            //            xtype: 'radiofield',
            //            name: 'us',
            //            inputValue: "u_group",
            //            id: "us1",
            //            checked: true,
            //            listeners: {
            //                change: function (radio, newValue, oldValue) {
            //                    //var rdo_group = Ext.getCmp("usr1");
            //                    //var rdo_groppset = Ext.getCmp("usr2");
            //                    var com_group = Ext.getCmp("groupname1");
            //                    var btn_group = Ext.getCmp("condi_set1");
            //                    if (newValue) {
            //                        btn_group.setDisabled(true);
            //                        com_group.setValue("0");
            //                        com_group.setDisabled(false);
            //                        com_group.allowBlank = false;
            //                        Ext.getCmp("userInfo1").hide();
            //                    }
            //                }
            //            }
            //        },
            //        {
            //            //會員群組
            //            xtype: 'combobox',
            //            editable: false,
            //            hidden: false,
            //            id: 'groupname1',
            //            name: 'groupname1',
            //            hiddenName: 'groupname1',
            //            colName: 'groupname1',
            //            store: VipGroupStore,
            //            displayField: 'group_name',
            //            valueField: 'group_id',
            //            typeAhead: true,
            //            forceSelection: false,
            //            value: "0"


            //        }
            //    ]
            //},
            //{
            //    xtype: 'fieldcontainer',
            //    fieldLabel: USERCONDI,
            //    combineErrors: true,
            //    layout: 'hbox',
            //    margins: '0 200 0 0',
            //    defaults: {
            //        width: 100,
            //        hideLabel: true
            //    },
            //    items: [
            //        {
            //            xtype: 'radiofield',
            //            name: 'us',
            //            inputValue: "condi_set1",
            //            id: "us2",
            //            listeners: {
            //                change: function (radio, newValue, oldValue) {
            //                    var com_group = Ext.getCmp("groupname1");
            //                    var btn_group = Ext.getCmp("condi_set1");
            //                    if (newValue) {

            //                        com_group.allowBlank = true;
            //                        com_group.setValue("");
            //                        com_group.isValid();

            //                        btn_group.setDisabled(false);
            //                        com_group.setDisabled(true);
            //                        if (condition_id != "") {

            //                            Ext.getCmp("userInfo1").show();
            //                        }

            //                    }
            //                }
            //            }
            //        },
            //        {
            //            xtype: 'button',
            //            text: CONDINTION,
            //            disabled: false,
            //            id: 'condi_set1',
            //            colName: 'condi_set1',
            //            handler: function () {
            //                if (conditionID != "") {
            //                    showUserSetForm(null, conditionID, "userInfo1");
            //                } else {
            //                    showUserSetForm(null, condition_id, "userInfo1");
            //                }
            //            }
            //        }
            //    ]
            //},
            {
                xtype: 'textareafield',
                name: 'textarea',
                id: 'userInfo1',
                readOnly: true,
                anchor: '100%',
                hidden: true,
                value: ShowConditionData(conditionID, "userInfo1"),
                listeners: {
                    change: function (textarea1, newValue, oldValue) {
                        var textArea = Ext.getCmp("userInfo1");
                        if (newValue != "" && oldValue != "") {
                            textArea.show();
                        }
                    }
                }
            },
              {
                  xtype: 'combobox',
                  fieldLabel: 'web site 設定',
                  hidden: false,
                  id: 'website1',
                  name: 'website1',
                  hiddenName: 'group_name',
                  colName: 'groupname',
                  store: SiteStore,
                  displayField: 'Site_Name',
                  valueField: 'Site_Id',
                  typeAhead: true,
                  forceSelection: false,
                  allowBlank: false,
                  editable: false,
                  multiSelect: true, //多選
                  queryMode: 'local',
                  emptyText: 'SELECT',
                  listeners: {
                      select: function (a, b, c) {
                          websiteID = Ext.getCmp('website1').getValue();
                      }
                  }
              },
               {
                   fieldLabel: '固定加購價',
                   xtype: 'numberfield',
                   padding: '10 0 5 0',
                   id: 'fixed_price1',
                   name: 'fixed_price1',
                   allowBlank: false,
                   labelWidth: 100,
        
                   minValue: 1
               },
                {
                    fieldLabel: '限購件數',
                    xtype: 'numberfield',
                    id: 'buy_limit1',
                    name: 'buy_limit1',
                    allowBlank: false, //允許為空
                    hidden: false,
                    minValue: 1,
                    maxValue: 9999

                    //fieldLabel: '限購件數',
                    //xtype: 'displayfield',
                    //id: 'buy_limit1',
                    //name: 'buy_limit1',
                    //readOnly: true,
                    //value: 1
                },
            {//促銷商品類別
                xtype: 'form',
                hidden: false,
                layout: 'hbox',
                border: 0,
                frame: true,
                id: 'p_sortpanl',
                colName: 'user_codi',
                items: [
                    {
                        xtype: 'label',
                        text: PROSORT,
                        id: 'p_sort',
                        margin: '0 8 0 0'
                    },
                    {
                        xtype: 'button',
                        text: '條件設定1',
                        width: 120,
                        id: 'p_condi_red',
                        colName: 'p_condi',
                        name: 'p_condi_red',
                        handler: function () {
                            var regex = /^([0-9]+,)*[0-9]+$/;
                            if (!regex.test(websiteID)) {
                                websiteID = SiteStore.getAt(0).get('Site_id')
                            }
                            PromationMationShow(produCateID_one, websiteID);
                        }
                    }
                    ,
                      {
                          xtype: 'button',
                          text: '條件設定2',
                          width: 120,
                          id: 'p_condi_green',
                          colName: 'p_condi',
                          name: 'p_condi_green',
                          handler: function () {
                              var regex = /^([0-9]+,)*[0-9]+$/;
                              if (!regex.test(websiteID)) {
                                  websiteID = SiteStore.getAt(0).get('Site_id')
                              }
                              PromationMationShow(produCateID_two, websiteID);
                          }
                      }
                ]
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'rd_deliver',
                name: 'rd_deliver',
                fieldLabel: YSCLASS,
                colName: 'rd_deliver',
                width: 400,
                defaults: {
                    name: 'rd_Deliver',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                    { boxLabel: '不分', id: 'bufen1', inputValue: '0', checked: true },
                    { boxLabel: '常溫', id: 'CW', inputValue: '1' },
                    { boxLabel: '冷藏', id: 'LC', inputValue: '2' }
                ]
            },
           
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'rd_device',
                name: 'rd_device',
                fieldLabel: DEVICE,
                colName: 'rd_device',
                width: 400,
                defaults: {
                    name: 'rd_Device',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                    { boxLabel: '不分', id: 'bufen', inputValue: '0', checked: true },
                    { boxLabel: 'PC', id: 'pc', inputValue: '1' },
                    { boxLabel: '手機/平板', id: 'mobilepad', inputValue: '4' }
                ]
            },
                {
                    xtype: "datetimefield",
                    fieldLabel: BEGINTIME,
                    id: 'start_date',
                    editable: false,
                    name: 'start_date',
                    anchor: '95%',
                    format: 'Y-m-d H:i:s',
                    width: 150,
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_date");
                            var end = Ext.getCmp("end_date");
                            var s_date = new Date(start.getValue());
                            var ttime = s_date;
                            ttime = new Date(ttime.setMonth(s_date.getMonth() + 1));
                            ttime = new Date(ttime.setMinutes(59));
                            ttime = new Date(ttime.setSeconds(59));
                            ttime = new Date(ttime.setHours(23));

                            end.setValue(ttime);
                        }
                    }
                },
                  {
                      xtype: "datetimefield",
                      fieldLabel: ENDTIME,
                      editable: false,
                      id: 'end_date',
                      anchor: '95%',
                      name: 'end_date',
                      format: 'Y-m-d H:i:s',
                      allowBlank: false,
                      submitValue: true,
                      value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                      listeners: {
                          select: function (a, b, c) {
                              var start = Ext.getCmp("start_date");
                              var end = Ext.getCmp("end_date");
                              var s_date = new Date(start.getValue());
                              var now_date = new Date(end.getValue());
                              if (end.getValue() < start.getValue()) {
                                  Ext.Msg.alert(INFORMATION, TIMETIP);
                              }
                              var ttime = now_date;
                              ttime = new Date(ttime.setMonth(now_date.getMonth()));
                              ttime = new Date(ttime.setMinutes(59));
                              ttime = new Date(ttime.setSeconds(59));
                              ttime = new Date(ttime.setHours(23));
                              end.setValue(ttime);
                          }
                      }
                  }
        ]
    });

    var allpan = new Ext.panel.Panel({
        width: 390,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            // 应用到所有子面板
            border: false
        },
        // 这里仅仅用几个按钮来示例一种可能的导航场景.
        bbar: [
        {
            id: 'move-prev',
            text: '上一步',
            handler: function (btn) {
                navigate(btn.up("panel"), "prev");
            },
            disabled: true
        },
        '->', // 一个长间隔, 使两个按钮分布在两边
        {
            id: 'move-next',
            text: '下一步',
            handler: function (btn) {
                navigate(btn.up("panel"), "next");
            }
        }
        ],
        // 布局下的各子面板 //firstForm,secondForm
        items: [firstForm, secondForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "不同品固定價",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [allpan],
        constrain: true,
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
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                //firstForm.getForm().loadRecord(row);
                //secondForm.getForm().loadRecord(row);
                firstForm.getForm().loadRecord();
                secondForm.getForm().loadRecord();
            }
        }
    });
    var UpdFrm = Ext.widget('form', {
        id: 'UpdFrm',
        frame: true,
        plain: true,
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        // url: '/PromoAdditionalPrice/PromoAdditionalPriceEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
                {
                    fieldLabel: '活動名稱',
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'event_name',
                    name: 'event_name',
                    allowBlank: false,
                    labelWidth: 80
                },
                {
                    fieldLabel: '活動描述',
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'event_desc',
                    name: 'event_desc',
                    allowBlank: false,
                    labelWidth: 80
                },
                {
                    xtype: 'radiogroup',
                    fieldLabel: ISBANNERURL,
                    id: 'url_by',
                    colName: 'url_by',
                    name: 'url_by',
                    defaults: {
                        name: 'url_by'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                    {
                        id: 'btr1',
                        boxLabel: YES,
                        inputValue: '1',
                        name: 'btr',
                        width: 100,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var banner_url = Ext.getCmp("banner_link_url");
                                var banner_image = Ext.getCmp("banner_image");
                                if (newValue) {
                                    banner_url.allowBlank = false;
                                    banner_url.show();
                                    banner_image.allowBlank = false;
                                    banner_image.show();
                                    banner_url.setValue(linkPath + event_id);
                                }
                            }
                        }
                    },
                    {
                        id: 'btr2',
                        boxLabel: NO,
                        checked: true,
                        name: 'btr',
                        inputValue: '0',
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var banner_url = Ext.getCmp("banner_link_url");
                                var banner_image = Ext.getCmp("banner_image");
                                if (newValue) {
                                    //                                banner_url.allowBlank = true;
                                    //                                banner_url.hide();
                                    //                                banner_url.setValue("");
                                    //                                banner_image.allowBlank = true;
                                    //                                banner_image.hide();
                                    banner_url.setValue("0");
                                    banner_url.allowBlank = true;
                                    banner_url.setValue("");
                                    banner_url.isValid();
                                    banner_url.hide();
                                    banner_image.setValue("0");
                                    banner_image.allowBlank = true;
                                    banner_image.setValue("");
                                    banner_image.isValid();
                                    banner_image.hide();
                                }
                            }
                        }
                    }]
                },
                {
                    xtype: 'textfield',
                    fieldLabel: SELFURL,
                    name: 'banner_link_url',
                    id: 'banner_link_url',
                    labelWidth: 80,
                    submitValue: true,
                    vtype: 'url',
                    allowBlank: true,
                    hidden: true
                },
                {
                    //專區Banner
                    xtype: 'filefield',
                    name: 'banner_image',
                    id: 'banner_image',
                    fieldLabel: '專區Banner',
                    labelWidth: 60,
                    msgTarget: 'side',
                    allowBlank: true,
                    anchor: '90%',
                    buttonText: '選擇...',
                    hidden: true,
                    fileUpload: true
                },
                //{//會員群組和會員條件二擇一
                //    xtype: 'fieldcontainer',
                //    fieldLabel: VIPGROUP,
                //    combineErrors: true,
                //    margins: '0 200 0 0',
                //    layout: 'hbox',
                //    defaults: {
                //        hideLabel: true,
                //        width: 100
                //    },
                //    items: [
                //        //{
                //        //    xtype: 'radiofield',
                //        //    name: 'usr',
                //        //    inputValue: "groupname",
                //        //    id: "usr1",
                //        //    checked: true,
                //        //    listeners: {
                //        //        change: function (radio, newValue, oldValue) {
                //        //            //                                var rdo_group = Ext.getCmp("us1");
                //        //            //                                var rdo_groppset = Ext.getCmp("us2");
                //        //            var com_group = Ext.getCmp("group_name");
                //        //            var btn_group = Ext.getCmp("condi_set");
                //        //            if (newValue) {
                //        //                btn_group.setDisabled(true);
                //        //                com_group.setValue("0");
                //        //                com_group.setDisabled(false);
                //        //                com_group.allowBlank = false;
                //        //                Ext.getCmp("userInfo").hide();
                //        //            }
                //        //        }

                //        //    }
                //        //},
                //        {//會員群組
                //            xtype: 'combobox',
                //            allowBlank: true,
                //            editable: false,
                //            hidden: false,
                //            id: 'group_name',
                //            name: 'group_name',
                //            hiddenName: 'group_name',
                //            colName: 'groupname',
                //            store: VipGroupStore,
                //            displayField: 'group_name',
                //            valueField: 'group_id',
                //            typeAhead: false,
                //            forceSelection: false
                //        }
                //    ]
                //},
                //{
                //    xtype: 'fieldcontainer',
                //    fieldLabel: USERCONDI,
                //    combineErrors: true,
                //    layout: 'hbox',
                //    margins: '0 200 0 0',
                //    defaults: {
                //        width: 100,
                //        hideLabel: true
                //    },
                //    items: [
                //        {
                //            xtype: 'radiofield',
                //            name: 'usr',
                //            inputValue: "condi_set",
                //            id: "usr2",
                //            listeners: {
                //                change: function (radio, newValue, oldValue) {
                //                    var com_group = Ext.getCmp("group_name");
                //                    var btn_group = Ext.getCmp("condi_set");
                //                    if (newValue) {

                //                        com_group.allowBlank = true;
                //                        com_group.setValue("");
                //                        com_group.isValid();

                //                        btn_group.setDisabled(false);
                //                        com_group.setDisabled(true);
                //                        if (condition_id != "" && condition_id != "0" && condition_id != 0) {
                //                            Ext.getCmp("userInfo").show();
                //                        }
                //                    }

                //                }
                //            }
                //        },
                //        {
                //            xtype: 'button',
                //            text: CONDINTION,
                //            disabled: false,
                //            id: 'condi_set',
                //            colName: 'condi_set',
                //            handler: function () {
                //                if (conditionID != "") {
                //                    showUserSetForm(null, conditionID, "userInfo");
                //                } else {
                //                    showUserSetForm(null, condition_id, "userInfo");
                //                }
                //            }

                //        }
                //    ]
                //},
                {
                    xtype: 'textareafield',
                    name: 'textarea',
                    id: 'userInfo',
                    anchor: '100%',
                    readOnly: true,
                    hidden: true,
                    value: ShowConditionData(conditionID, "userInfo"),
                    listeners: {
                        change: function (textarea, newValue, oldValue) {
                            var textArea = Ext.getCmp("userInfo");
                            if (newValue != "" && oldValue != "") {
                                textArea.show();
                            }
                        }
                    }
                },
                  {
                      xtype: 'combobox',
                      fieldLabel: 'web site 設定',
                      hidden: false,
                      id: 'website',
                      name: 'website',
                      hiddenName: 'website',
                      colName: 'groupname',
                      store: SiteStore,
                      displayField: 'Site_Name',
                      valueField: 'Site_Id',
                      typeAhead: true,
                      forceSelection: false,
                      allowBlank: false,
                      editable: false,
                      multiSelect: true, //多選
                      queryMode: 'local',
                      emptyText: 'SELECT',
                      listeners: {
                          select: function (a, b, c) {
                              websiteID = Ext.getCmp('website').getValue();
                          }
                      }
                  },
                  {
                      fieldLabel: '固定加購價',
                      xtype: 'numberfield',
                      padding: '10 0 5 0',
                      id: 'fixed_price',
                      name: 'fixed_price',
                     
                      allowBlank: false,
                      labelWidth: 100,
                      minValue: 1
                  },
                   {
                       fieldLabel: '限購件數',
                       xtype: 'numberfield',
                       id: 'buy_limit',
                       name: 'buy_limit',
                       allowBlank: false, //允許為空
                       hidden: false,
                       minValue: 1,
                       maxValue: 9999

                       //fieldLabel: '限購件數',
                       //xtype: 'displayfield',
                       //id: 'buy_limit',
                       //name: 'buy_limit',
                       //readOnly: true,
                   },
                {//促銷商品類別
                    xtype: 'form', //類別設定
                    hidden: false,
                    layout: 'hbox',
                    border: 0,
                    frame: true,
                    id: 'p_sortpanl1',
                    colName: 'user_codi',
                    items: [
                            {
                                xtype: 'label',
                                text: '促銷商品類別',
                                id: 'p_sort',
                                margin: '0 8 0 0'
                            },
                            {
                                xtype: 'button',
                                text: '條件設定1',
                                width: 120,
                                id: 'p_condi_red1',
                                colName: 'p_condi',
                                name: 'p_condi_red',
                                handler: function () {
                                    var regex = /^([0-9]+,)*[0-9]+$/;
                                    if (!regex.test(websiteID)) {
                                        websiteID = SiteStore.getAt(0).get('Site_id')
                                    }
                                    PromationMationShow(produCateID_one, websiteID);
                                }
                            }
                            ,
                            {
                                xtype: 'button',
                                text: '條件設定2',
                                width: 120,
                                id: 'p_condi_green1',
                                colName: 'p_condi',
                                name: 'p_condi_green',
                                handler: function () {
                                    var regex = /^([0-9]+,)*[0-9]+$/;
                                    if (!regex.test(websiteID)) {
                                        websiteID = SiteStore.getAt(0).get('Site_id')
                                    }
                                    PromationMationShow(produCateID_two, websiteID);
                                }

                            }

                    ]

                },
                {
                    xtype: 'radiogroup',
                    hidden: false,
                    id: 'deliver_type',
                    name: 'deliver_type',
                    fieldLabel: '運送類別',
                    colName: 'deliver_type',
                    width: 400,
                    defaults: {
                        name: 'Deliver',
                        margin: '0 8 0 0'
                    },
                    columns: 3,
                    vertical: true,
                    items: [
                            { boxLabel: '不分', id: 'bufen2', inputValue: '0', checked: true },
                            { boxLabel: '常溫', id: 'CW1', inputValue: '1' },
                            { boxLabel: '冷藏', id: 'LC1', inputValue: '2' }
                    ]
                },
                {
                    xtype: 'radiogroup', //單選框
                    hidden: false, //是否隱藏
                    id: 'device',
                    name: 'device',
                    fieldLabel: '裝置', //控件顯示的字體
                    colName: 'device', //
                    width: 400, //設置的寬度
                    defaults: {
                        name: 'Device',
                        margin: '0 8 0 0'//樣式
                    },
                    columns: 3, //設置顯示的三個單選按鈕
                    vertical: true,
                    items: [
                        { boxLabel: '不分', id: 'bufen3', inputValue: '0', checked: true },
                        { boxLabel: 'PC', id: 'pc1', inputValue: '1' },
                        { boxLabel: '手機/平板', id: 'mobilepad1', inputValue: '4' }]
                },

               
                      {
                          xtype: "datetimefield",

                          fieldLabel: BEGINTIME,
                          id: 'starts',
                          editable: false,
                          name: 'starts',
                          anchor: '95%',
                          format: 'Y-m-d H:i:s',
                          width: 150,
                          allowBlank: false,
                          submitValue: true,
                          value: Tomorrow(),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("starts");
                                  var end = Ext.getCmp("end");
                                  var s_date = new Date(start.getValue());
                                  var ttime = s_date;
                                  ttime = new Date(ttime.setMonth(s_date.getMonth() + 1));
                                  ttime = new Date(ttime.setMinutes(59));
                                  ttime = new Date(ttime.setSeconds(59));
                                  ttime = new Date(ttime.setHours(23));

                                  end.setValue(ttime);
                              }
                          }
                      },
                      {
                          xtype: "datetimefield",
                          fieldLabel: ENDTIME,
                          editable: false,
                          id: 'end',
                          anchor: '95%',
                          name: 'end',
                          format: 'Y-m-d H:i:s',
                          allowBlank: false,
                          submitValue: true,
                          value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("starts");
                                  var end = Ext.getCmp("end");
                                  var s_date = new Date(start.getValue());
                                  var now_date = new Date(end.getValue());
                                  if (end.getValue() < start.getValue()) {
                                      Ext.Msg.alert(INFORMATION, TIMETIP);
                                  }
                                  var ttime = now_date;
                                  ttime = new Date(ttime.setMonth(now_date.getMonth()));
                                  ttime = new Date(ttime.setMinutes(59));
                                  ttime = new Date(ttime.setSeconds(59));
                                  ttime = new Date(ttime.setHours(23));
                                  end.setValue(ttime);
                              }
                          }
                      }

        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();

                //if ((condition_id == "" || condition_id == "0") && (Ext.getCmp("group_name").getValue() == null || Ext.getCmp("group_name").getValue() == "")) {
                //    Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                //    return;
                //}
                if (form.isValid()) {
                    var bool = 0;
                    websiteID = Ext.htmlEncode(Ext.getCmp('website').getValue())
                    Ext.Ajax.request({
                        url: '/PromoAdditionalPrice/DeletLessThen',
                        method: 'post',
                        params: { rowid: promoID, fixed_price: Ext.htmlEncode(Ext.getCmp('fixed_price').getValue()), producateid: produCateID_two, websiteid: websiteID, types: 2 },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {

                                if (result.delcount == 1) {
                                    Ext.Msg.alert("提示", "低於商品加構價的將會刪除!");
                                    setTimeout(2000);
                                }
                                
                                Ext.Ajax.request({//-----
                                    method: 'post',
                                    url: '/PromoAdditionalPrice/PromoAdditionalPriceEdit',
                                    params: {
                                        rowid: promoID,
                                        categoryid: produCateID,
                                        event_name: Ext.getCmp('event_name').getValue(),
                                        event_desc: Ext.getCmp('event_desc').getValue(),
                                        banner_url: Ext.htmlEncode(Ext.getCmp('banner_link_url').getValue()),
                                        banner: Ext.htmlEncode(Ext.getCmp('banner_image').getValue()),
                                        // group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                                        group_id:0,
                                        condition_id: condition_id,
                                        deliver_id: Ext.htmlEncode(Ext.getCmp("deliver_type").getValue().Deliver),
                                        device_id: Ext.htmlEncode(Ext.getCmp("device").getValue().Device),
                                        fixed_price: Ext.htmlEncode(Ext.getCmp("fixed_price").getValue()),
                                        buy_limit: Ext.htmlEncode(Ext.getCmp("buy_limit").getValue()),
                                        starts: Ext.Date.format(new Date(Ext.getCmp('starts').getValue()), 'Y-m-d H:i:s'),
                                        end: Ext.Date.format(new Date(Ext.getCmp('end').getValue()), 'Y-m-d H:i:s'),//Ext.htmlEncode(Ext.getCmp('end').getValue()),
                                        //starts: Ext.htmlEncode(Ext.getCmp('starts').getValue()),
                                        //end: Ext.htmlEncode(Ext.getCmp('end').getValue()),
                                        side: Ext.htmlEncode(Ext.getCmp('website').getValue())

                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        //var result = Ext.decode(action.response.responseText);
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        if (result.success) {
                                            FaresStore.load();
                                            Updwin.close();
                                        }
                                        else {
                                            if (result.msg.toString() == "1") {
                                                Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                            }
                                            else if (result.msg.toString() == "2") {
                                                Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                            }
                                            else if (result.msg.toString() == "3") {
                                                Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
                                            }
                                            else {
                                                Ext.Msg.alert(INFORMATION, FAILURE);
                                            }
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.msg.toString() == "1") {
                                            Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                        }
                                        else if (result.msg.toString() == "2") {
                                            Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                        }
                                        else if (result.msg.toString() == "3") {
                                            Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    }
                                });


                            } else {
                                Ext.Msg.alert(INFORMATION, "操作失敗1");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "操作失敗2");
                        }
                    });

                    //form.submit({
                    //    params: {
                    //        rowid: promoID,
                    //        categoryid: produCateID,
                    //        event_name: Ext.getCmp('event_name').getValue(),
                    //        event_desc: Ext.getCmp('event_desc').getValue(),
                    //        banner_url: Ext.htmlEncode(Ext.getCmp('banner_link_url').getValue()),
                    //        banner: Ext.htmlEncode(Ext.getCmp('banner_image').getValue()),
                    //        group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                    //        condition_id: condition_id,
                    //        deliver_id: Ext.htmlEncode(Ext.getCmp("deliver_type").getValue().Deliver),
                    //        device_id: Ext.htmlEncode(Ext.getCmp("device").getValue().Device),
                    //        fixed_price: Ext.htmlEncode(Ext.getCmp("fixed_price").getValue()),
                    //        buy_limit: Ext.htmlEncode(Ext.getCmp("buy_limit").getValue()),
                    //        starts: Ext.htmlEncode(Ext.getCmp('starts').getValue()),
                    //        end: Ext.htmlEncode(Ext.getCmp('end').getValue()),
                    //        side: Ext.htmlEncode(Ext.getCmp('website').getValue())

                    //    },
                    //    success: function (form, action) {
                    //        var result = Ext.decode(action.response.responseText);
                    //        Ext.Msg.alert(INFORMATION, SUCCESS);
                    //        if (result.success) {
                    //            FaresStore.load();
                    //            Updwin.close();
                    //        }
                    //        else {
                    //            Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                    //        }
                    //    },
                    //    failure: function (form, action) {
                    //        var result = Ext.decode(action.response.responseText);
                    //        if (result.msg.toString() == "1") {
                    //            Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                    //        }
                    //        else if (result.msg.toString() == "2") {
                    //            Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                    //        }
                    //        else if (result.msg.toString() == "3") {
                    //            Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
                    //        }
                    //        else {
                    //            Ext.Msg.alert(INFORMATION, FAILURE);
                    //        }
                    //    }
                    //});
                }
            }
        }]
    });
    var Updwin = Ext.create('Ext.window.Window', {
        title: "不同品固定價",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [UpdFrm],
        constrain: true,
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
                             Updwin.destroy();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                UpdFrm.getForm().loadRecord(row);
                promoID = row.data.id;
                var devi = row.data.device;
                produCateID = row.data.category_id;
                produCateID_one = row.data.left_category_id;
                produCateID_two = row.data.right_category_id;
                var a = row.data.fixed_price;
                var b = row.data.buy_limit;
                event_id = row.data.event_id;
                var siteIDs = row.data.website.toString().split(',');
                var combobox = Ext.getCmp('website');
                //var store = combobox.store;
                var arrTemp = new Array();
                // var length = store.count();
                for (var i = 0; i < siteIDs.length; i++) {
                    //arrTemp.push(store.getAt(store.find("Site_Id", siteIDs[i])));
                    arrTemp.push(siteIDs[i]);
                }
                combobox.setValue(arrTemp);
                //修改時對site賦值end
                Ext.getCmp('banner_image').setRawValue(row.data.banner_image);
                var _grop = row.data.group_name;
                //if (row.data.condition_id != 0) {
                //    Ext.getCmp('usr1').setValue(false);
                //    Ext.getCmp('usr2').setValue(true);
                //}
                //else {
                //    Ext.getCmp('usr1').setValue(true);
                //    Ext.getCmp('usr2').setValue(false);
                //    if (row.data.group_name == "" || row.data.group_name == null) {
                //        Ext.getCmp('group_name').setValue("0");
                //    }
                //    else {
                //        Ext.getCmp('group_name').setValue(row.data.group_name);
                //    }
                //}

                var deli = row.data.deliver_type;
                if (devi == 0) {
                    Ext.getCmp("bufen3").setValue(true);
                    Ext.getCmp("pc1").setValue(false);
                    Ext.getCmp("mobilepad1").setValue(false);
                } else if (devi == 1) {
                    Ext.getCmp("bufen3").setValue(false);
                    Ext.getCmp("pc1").setValue(true);
                    Ext.getCmp("mobilepad1").setValue(false);
                } else if (devi == 4) {
                    Ext.getCmp("bufen3").setValue(false);
                    Ext.getCmp("pc1").setValue(false);
                    Ext.getCmp("mobilepad1").setValue(true);
                }
                if (deli == 0) {
                    Ext.getCmp("bufen2").setValue(true);
                    Ext.getCmp("CW1").setValue(false);
                    Ext.getCmp("LC1").setValue(false);
                }
                else if (deli == 1) {
                    Ext.getCmp("bufen2").setValue(false);
                    Ext.getCmp("CW1").setValue(true);
                    Ext.getCmp("LC1").setValue(false);
                }
                else if (deli == 2) {
                    Ext.getCmp("bufen2").setValue(false);
                    Ext.getCmp("CW1").setValue(false);
                    Ext.getCmp("LC1").setValue(true);
                }
                if (row.data.url_by == 0) {
                    Ext.getCmp('btr1').setValue(false);
                    Ext.getCmp('btr2').setValue(true);
                    Ext.getCmp("banner_link_url").setValue("");
                }
                else if (row.data.url_by == 1) {
                    Ext.getCmp('btr1').setValue(true);
                    Ext.getCmp('btr2').setValue(false);
                    var img = row.data.banner_image.toString();
                    var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
                    Ext.getCmp('banner_image').setRawValue(imgUrl);
                    var url = row.data.category_link_url.toString();
                    Ext.getCmp("banner_link_url").setValue(url);
                }
            }
        }
    });
    if (row != null) {

        Updwin.show();

    } else {

        editWin.show();

    }

}


