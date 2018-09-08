variable "resource_group_name" { }

variable "storage_account_name" { }

variable "location" {
  default = "westus2"
}

resource "azurerm_resource_group" "group" {
  name     = "${var.resource_group_name}"
  location = "${var.location}"
}

resource "azurerm_storage_account" "storageaccount" {
  name                     = "${var.storage_account_name}"
  resource_group_name      = "${azurerm_resource_group.group.name}"
  location                 = "${azurerm_resource_group.group.location}"
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "zipped_data" {
  name                  = "zipped-data"
  resource_group_name   = "${azurerm_resource_group.group.name}"
  storage_account_name  = "${azurerm_storage_account.storageaccount.name}"
  container_access_type = "private"
}

resource "azurerm_storage_container" "unzipped_data" {
  name                  = "unzipped-data"
  resource_group_name   = "${azurerm_resource_group.group.name}"
  storage_account_name  = "${azurerm_storage_account.storageaccount.name}"
  container_access_type = "private"
}

resource "azurerm_storage_queue" "entries" {
  name                  = "entries"
  resource_group_name   = "${azurerm_resource_group.group.name}"
  storage_account_name  = "${azurerm_storage_account.storageaccount.name}"
}

resource "azurerm_storage_table" "table" {
  name                 = "sampledata"
  resource_group_name   = "${azurerm_resource_group.group.name}"
  storage_account_name  = "${azurerm_storage_account.storageaccount.name}"
}