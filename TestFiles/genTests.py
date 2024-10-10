import torch
from safetensors.torch import save_file

tensors = {
    "embedding": torch.zeros((2, 2)),
    "attention": torch.tensor([[1,2,3],[4,5,6]],dtype=torch.int8)
}
metadata={"key1":"value1","key2":"value2"}
save_file(tensors, "basic_model.safetensors")
save_file(tensors, "with_metadata.safetensors",metadata)