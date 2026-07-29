terraform {
  required_providers {
    oci = {
      source  = "oracle/oci"
      version = ">= 5.0.0"
    }
  }
}

provider "oci" {
  region               = var.region
  tenancy_ocid         = var.tenancy_ocid
  user_ocid            = var.user_ocid
  fingerprint          = var.fingerprint
  private_key_path     = var.private_key_path
}

data "oci_identity_availability_domains" "ads" {
  compartment_id = var.tenancy_ocid
}

# Fetch the correct ARM-based Oracle Linux image
data "oci_core_images" "arm_images" {
  compartment_id           = var.compartment_ocid
  operating_system         = "Oracle Linux"
  operating_system_version = "8" # Or 9
  shape                    = "VM.Standard.A1.Flex"
  sort_by                  = "TIMECREATED"
  sort_order               = "DESC"
}

resource "oci_core_instance" "microservices_vm" {
  compartment_id      = var.compartment_ocid
  availability_domain = data.oci_identity_availability_domains.ads.availability_domains[0].name
  shape               = "VM.Standard.A1.Flex"

  shape_config {
    ocpus         = 1 # Up to 4 for free tier
    memory_in_gbs = 6 # Up to 24 for free tier
  }

  source_details {
    source_type = "image"
    source_id   = data.oci_core_images.arm_images.images[0].id # Uses the ARM image we queried
  }

  create_vnic_details {
    subnet_id                 = oci_core_subnet.public_subnet.id
    assign_public_ip          = true # Crucial: Gives it a public IP
    assign_private_dns_record = true
    hostname_label            = "microservicesvm"
  }

  metadata = {
    # Injects your SSH key into the VM so you can log in
    ssh_authorized_keys = file(var.ssh_public_key_path)
  }

  display_name = "microservices-vm"
  
  # Preserve the boot volume if we destroy the instance, to save time on rebuilds
  preserve_boot_volume = false 
}