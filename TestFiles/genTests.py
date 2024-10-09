import torch
from safetensors.torch import save_file

tensors = {
    "embedding": torch.zeros((2, 2)),
    "attention": torch.zeros((2, 3))
}
metadata={"key1":"value1","key2":"value2"}
save_file(tensors, "basic_model.safetensors")
save_file(tensors, "with_metadata.safetensors",metadata)