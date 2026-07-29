variable "tenancy_ocid" {
  description = "The OCID of the OCI Tenancy"
  type        = string
}

variable "user_ocid" {
  description = "The OCID of the OCI User"
  type        = string
}

variable "fingerprint" {
  description = "The fingerprint of the API signing key"
  type        = string
}

variable "private_key_path" {
  description = "The path to the private API signing key"
  type        = string
}

variable "region" {
  description = "The OCI region (e.g., eu-frankfurt-1)"
  type        = string
}

variable "compartment_ocid" {
  description = "The OCID of the compartment where resources will be created"
  type        = string
}

variable "ssh_public_key_path" {
  description = "The path to your public SSH key for the compute instance"
  type        = string
}